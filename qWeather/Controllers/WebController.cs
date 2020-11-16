using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using qWeather.Context;
using qWeather.Models;
using qWeather.Models.ESP8266;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.DynamicData;
using System.Web.Http;

namespace qWeather.Controllers
{
    [RoutePrefix("api")]
    public class WebController : ApiController
    {
        public WeatherDbContext context = new WeatherDbContext();

        [Route("")]
        [HttpGet()]
        public async Task<IEnumerable<Weather>> Get()
        {
            return await context.Weather.OrderByDescending(x => x.DATETIME).ToListAsync();
        }

        [Route("")]
        [HttpGet()]
        public async Task<IEnumerable<Weather>> Get([FromUri]DateTime start, [FromUri]DateTime end)
        {
            return await context.Weather.Where(x => x.DATETIME >= start && x.DATETIME <= end).OrderBy(x => x.DATETIME).ToListAsync();
        }

        [Route("last")]
        [HttpGet()]
        public async Task<Weather> GetLast()
        {
            DateTime LastDate = await context.Weather.MaxAsync(y => y.DATETIME);
            return await context.Weather.Where(x => x.DATETIME == LastDate).OrderByDescending(x => x.DATETIME).FirstOrDefaultAsync();
        }
        
        [Route("now")]
        [HttpGet]
        public async Task<Weather> Now()
        {
            ESPData ESPData = new ESPData();
            string URL = WebConfigurationManager.AppSettings["ESP8266url"];
            ESPData = await ESPData.GetAsync(new Uri(URL));

            return new Weather
            {
                Id = ESPData.id,
                DATETIME = DateTime.Now,
                VAL1 = ESPData.variables.T_OUT,
                VAL2 = ESPData.variables.T_IN,
                HUMIDITY = ESPData.variables.Humidity
            };
        }

        [Route("average")]
        [HttpGet()]
        public async Task<WeatherAverageView> GetAverage([FromUri]DateTime start, [FromUri]DateTime end)
        {            
            var weathers = await context.Weather.Where(x => x.DATETIME >= start && x.DATETIME <= end).ToListAsync();

            return new WeatherAverageView(weathers);
        }

        [Route("averagebyday")]
        [HttpGet()]
        public async Task<WeatherAverageView> GetAverageByDay([FromUri] DateTime date)
        {
            var start = date.AddHours(-24);
            var weathers = await context.Weather.Where(x => x.DATETIME >= start && x.DATETIME <= date).ToListAsync();

            return new WeatherAverageView(weathers);
        }

        [Route("averagebyweek")]
        [HttpGet]
        public async Task<WeatherAverageView> GetAverageByWeek([FromUri] DateTime date)
        {
            var start = date.StartOfWeek();
            var end = date.EndOfWeek();
            var weathers = await context.Weather.Where(x => x.DATETIME >= start && x.DATETIME <= end).ToListAsync();

            return new WeatherAverageView(weathers);
        }

        [Route("averagebymonth")]
        [HttpGet()]
        public async Task<WeatherAverageView> GetAverageByMonth([FromUri] DateTime date)
        {
            var start = new DateTime(date.Year, date.Month, 1);
            var weathers = await context.Weather.Where(x => x.DATETIME >= start && x.DATETIME <= date).ToListAsync();

            return new WeatherAverageView(weathers);
        }

        [Route("averagebyyear")]
        [HttpGet()]
        public async Task<WeatherAverageView> GetAverageByYear([FromUri] DateTime date)
        {
            var start = new DateTime(date.Year, 1, 1);
            var weathers = await context.Weather.Where(x => x.DATETIME >= start && x.DATETIME <= date).ToListAsync();

            return new WeatherAverageView(weathers);
        }

        [Route("update")]
        [HttpPost]
        public async Task UpdateSensors(ESPSensors sensors)
        {
            try
            {
                context.Weather.Add(new Weather
                {
                    DATETIME = DateTime.Now,
                    VAL1 = sensors.T_OUT,
                    VAL2 = sensors.T_IN,
                    HUMIDITY = sensors.Humidity
                });

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                logging.WriteLog(new string[]
                {
                    "error while inserting!",
                     ex.Message + " ### " + ex.InnerException
                });
            }
        }

        [Route("day")]
        [HttpGet()]
        public async Task<IEnumerable<Weather>> GetByDay([FromUri] DateTime date)
        {
            var start = date.AddDays(-1);

            var weatherGroups = await context.Weather.Where(x => x.DATETIME>= start &&
                                                                 x.DATETIME <= date)
                                                                    .GroupBy(x => x.DATETIME.Hour).ToListAsync();

            List<Weather> weathers = new List<Weather>();
            
            foreach(var item in weatherGroups)
            {
                var weatherByHour = weatherGroups.Where(x => x.Key == item.Key);
                foreach (var itemByHour in weatherByHour)
                {
                    weathers.Add(new Weather
                    {
                        DATETIME = new DateTime(date.Year, date.Month, itemByHour.FirstOrDefault().DATETIME.Day, item.Key, 0, 0),
                        VAL1 = itemByHour.Sum(x => x.VAL1) / itemByHour.Count(),
                        VAL2 = itemByHour.Sum(x => x.VAL2) / itemByHour.Count(),
                        HUMIDITY = itemByHour.Sum(x => x.HUMIDITY) / itemByHour.Count()
                    });
                }
            }

            return weathers.OrderBy(x=>x.DATETIME);
        }

        [Route("week")]
        [HttpGet()]
        public async Task<IEnumerable<Weather>> GetByWeek([FromUri] DateTime date)
        {
            var start = date.StartOfWeek();
            var end = date.EndOfWeek();

            var weatherGroups = await context.Weather.Where(x => x.DATETIME >= start && 
                                                                 x.DATETIME <= end)
                                                                .GroupBy(x=>x.DATETIME.Day).ToListAsync();

            List<Weather> weathers = new List<Weather>();

            foreach (var item in weatherGroups)
            {
                var weatherByDay = weatherGroups.Where(x => x.Key == item.Key);
                foreach(var itemByDay in weatherByDay)
                {
                    weathers.Add(new Weather
                    {
                        DATETIME = new DateTime(date.Year, date.Month, item.Key),
                        VAL1 = itemByDay.Sum(x=>x.VAL1) / itemByDay.Count(),
                        VAL2 = itemByDay.Sum(x=>x.VAL2) / itemByDay.Count(),
                        HUMIDITY = itemByDay.Sum(x=>x.HUMIDITY) / itemByDay.Count()
                    });
                }
            }

            return weathers;
        }

        [Route("month")]
        [HttpGet()]
        public async Task<IEnumerable<Weather>> GetByMonth([FromUri] DateTime date)
        {
            var start = new DateTime(date.Year, date.Month, 1);
            
            var weatherGroups = await context.Weather.Where(x => x.DATETIME >= start && 
                                                                 x.DATETIME <= date)
                                                                    .GroupBy(x => x.DATETIME.Day).ToListAsync();

            List<Weather> weathers = new List<Weather>();

            foreach(var item in weatherGroups)
            {
                var weatherByDay = weatherGroups.Where(x => x.Key == item.Key);
                foreach (var itemByDay in weatherByDay)
                {
                    weathers.Add(new Weather
                    {
                        DATETIME = new DateTime(date.Year, date.Month, item.Key),
                        VAL1 = itemByDay.Sum(x => x.VAL1) / itemByDay.Count(),
                        VAL2 = itemByDay.Sum(x => x.VAL2) / itemByDay.Count(),
                        HUMIDITY = itemByDay.Sum(x => x.HUMIDITY) / itemByDay.Count()
                    });
                }
            }

            return weathers;
        }

        [Route("year")]
        [HttpGet()]
        public async Task<IEnumerable<Weather>> GetByYear([FromUri] DateTime date)
        {
            var start = new DateTime(date.Year, 1, 1);

            var weatherGroups = await context.Weather.Where(x => x.DATETIME >= start &&
                                                               x.DATETIME <= date)
                                                                    .GroupBy(x => x.DATETIME.Month).ToListAsync();
            List<Weather> weathers = new List<Weather>();

            foreach(var item in weatherGroups)
            {
                var weatherByMonth = weatherGroups.Where(x => x.Key == item.Key);
                foreach(var itemByMonth in weatherByMonth)
                {
                    weathers.Add(new Weather
                    {
                        DATETIME = new DateTime(date.Year, item.Key, 1),
                        VAL1 = itemByMonth.Sum(x => x.VAL1) / itemByMonth.Count(),
                        VAL2 = itemByMonth.Sum(x => x.VAL2) / itemByMonth.Count(),
                        HUMIDITY = itemByMonth.Sum(x => x.HUMIDITY) / itemByMonth.Count()
                    });
                }
            }

            return weathers;
        }
    }
}