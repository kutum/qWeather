using Quartz;
using qWeather.Context;
using System;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace qWeather.Models.ESP8266
{
    public class ESPDataSender : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            using (var weatherDbcontext = new WeatherDbContext())
            {
                ESPData ESPData = new ESPData();
                string URL = WebConfigurationManager.AppSettings["ESP8266url"];
                ESPData = await ESPData.GetAsync(new Uri(URL));

                weatherDbcontext.Weather.Add(new Weather
                {
                    DATETIME = DateTime.Now,
                    VAL1 = ESPData.variables.T_OUT,
                    VAL2 = ESPData.variables.T_IN,
                    HUMIDITY = (int)ESPData.variables.Humidity
                });

                await weatherDbcontext.SaveChangesAsync();
            }
        }
    }
}
