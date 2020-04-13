using qWeather.Context;
using qWeather.Models;
using qWeather.Models.ESP8266;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
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
            return await context.Weather.Where(x => x.DATETIME >= start && x.DATETIME <= end).OrderByDescending(x => x.DATETIME).ToListAsync();
        }

        [Route("last")]
        [HttpGet()]
        public async Task<Weather> GetLast()
        {
            if (await context.Weather.AnyAsync())
            {
                DateTime LastDate = await context.Weather.MaxAsync(y => y.DATETIME);
                return await context.Weather.Where(x => x.DATETIME == LastDate).OrderByDescending(x => x.DATETIME).FirstOrDefaultAsync();
            }
            else
                throw new Exception("Empty database");
        }
        
        [Route("now")]
        [HttpGet]
        public async Task<Weather> Now()
        {
            ESPData ESPData = new ESPData();
            string URL = WebConfigurationManager.AppSettings["ESP8266url"];
            ESPData = await ESPData.GetAsync(new Uri(URL));

            Weather weather = new Weather
            {
                Id = ESPData.id,
                DATETIME = DateTime.Now,
                VAL1 = ESPData.variables.T_OUT,
                VAL2 = ESPData.variables.T_IN,
                HUMIDITY = (int)ESPData.variables.Humidity
            };

            return weather;
        }

        [Route("average")]
        [HttpGet()]
        public async Task<WeatherAverageView> GetAverage([FromUri]DateTime start, [FromUri]DateTime end)
        {
            if (!await context.Weather.AnyAsync())
            {
                throw new Exception("Empty database");
            }
                var weather = await context.Weather.Where(x => x.DATETIME >= start && x.DATETIME <= end).ToListAsync();

            float? insideSum = 0.0f;
            float? outsideSum = 0.0f;

            foreach (var item in weather)
            {
                insideSum += item.VAL2;
                outsideSum += item.VAL1;
            }

            WeatherAverageView weatherAverageView = new WeatherAverageView() {
                datetimefrom = start.ToString("yyyy-MM-dd HH:mm:ss"),
                datetimeend = end.ToString("yyyy-MM-dd HH:mm:ss"),
                insideTemp = (float)insideSum / weather.Count,
                outsideTemp = (float)outsideSum / weather.Count
            };

            return weatherAverageView;
        }
    }
}