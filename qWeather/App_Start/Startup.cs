using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(qWeather.Startup))]
namespace qWeather
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}