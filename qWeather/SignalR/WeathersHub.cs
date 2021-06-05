using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using qWeather.Context;

namespace qWeather.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    public class WeathersHub : Hub
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HubMethodName("sendMessages")]
        public static void SendMessages()
        {
            using (WeatherDbContext context = new WeatherDbContext())
            {
                IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<WeathersHub>();
                hubContext.Clients.All.updateMessages();
            }
        }
    }
}