﻿using qWeather.Models;
using qWeather.Models.ESP8266;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace qWeather
{
    public class MvcApplication : HttpApplication
    {

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ESPDataStartSend.Start();


            TelegramBot telegramBot = new TelegramBot();

            telegramBot.Bot.OnMessage += telegramBot.Bot_OnMessage;
            telegramBot.Bot.StartReceiving();
        }

        
    }
}
