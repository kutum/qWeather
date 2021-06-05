using qWeather.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace qWeather
{
    public class MvcApplication : HttpApplication
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            SqlDependency.Start(connectionString);

            try
            {
                TelegramBot telegramBot = new TelegramBot();
                telegramBot.Bot.OnMessage += telegramBot.Bot_OnMessage;
                telegramBot.Bot.StartReceiving();
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                logging.WriteLog(ex.Message);
            }
        }

        protected void Application_End()
        {
            SqlDependency.Stop(connectionString);
        }
    }
}
