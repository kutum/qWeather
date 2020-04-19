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
            if (Convert.ToBoolean(WebConfigurationManager.AppSettings["ESPScheduler"]))
            {
                try
                {
                    Logging logging = new Logging();

                    logging.WriteLog(new string[] { "start insert job" });

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

                        logging.WriteLog(new string[]
                        {
                            "insert",
                            "T_OUT = " + ESPData.variables.T_OUT,
                            "T_IN = " + ESPData.variables.T_IN,
                            "HUMIDITY = " + ESPData.variables.Humidity
                        });

                        await weatherDbcontext.SaveChangesAsync();
                        logging.WriteLog(new string[] { "Successfull insert!" });
                    }
                }
                catch (Exception ex)
                {
                    Logging logging = new Logging();

                    logging.WriteLog(new string[]
                    {
                        "error while inserting!",
                        "ESPDataSender.cs " + ex.Message + " ### " + ex.InnerException
                    });

                    await Execute(context);
                }
            }
        }
    }
}
