using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
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
            List<Weather> weathers = await context.Weather.Where(x => x.DATETIME >= start && x.DATETIME <= end).OrderBy(x=>x.DATETIME).ToListAsync();

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
    }
}