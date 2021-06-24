using Microsoft.AspNet.SignalR;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace qWeather
{
    public class MvcApplication : HttpApplication
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            GlobalHost.Configuration.TransportConnectTimeout = TimeSpan.FromSeconds(Convert.ToInt32(WebConfigurationManager.AppSettings["signalrTimeout"]));
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            SqlDependency.Start(connectionString);
        }

        protected void Application_End()
        {
            SqlDependency.Stop(connectionString);
        }
    }
}
