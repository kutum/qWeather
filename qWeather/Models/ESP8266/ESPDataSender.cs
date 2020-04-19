using Quartz;
using qWeather.Context;
using System;
using System.Data.Entity.Infrastructure;
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
                    using (var weatherDbcontext = new WeatherDbContext())
                    {
                        Logging logging = new Logging();

                        logging.WriteLog(new string[] { "start insert job" });

                        ESPData espdata = new ESPData();
                        string URL = WebConfigurationManager.AppSettings["ESP8266url"];
                        espdata = await espdata.GetAsync(new Uri(URL));

                        weatherDbcontext.Weather.Add(new Weather
                        {
                            DATETIME = DateTime.Now,
                            VAL1 = espdata.variables.T_OUT,
                            VAL2 = espdata.variables.T_IN,
                            HUMIDITY = espdata.variables.Humidity
                        });

                        try
                        {
                            await weatherDbcontext.SaveChangesAsync();

                            logging.WriteLog(new string[]
                            {
                                "Successfull insert!",
                                "insert at " + DateTime.Now.ToString(),
                                "T_OUT = " + espdata.variables.T_OUT,
                                "T_IN = " + espdata.variables.T_IN,
                                "HUMIDITY = " + espdata.variables.Humidity
                            });
                        }
                        catch (Exception ex)
                        {
                            logging.WriteLog(new string[]
                            {
                                "error while SaveChangesAsync()",
                                "ESPDataSender.cs " + ex.Message + " ### " + ex.InnerException
                            });
                            await Execute(context);
                        }

                        
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
