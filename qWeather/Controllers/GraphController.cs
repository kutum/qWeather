using qWeather.Context;
using qWeather.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using RouteAttribute = System.Web.Http.RouteAttribute;
using RoutePrefixAttribute = System.Web.Http.RoutePrefixAttribute;

namespace qWeather.Controllers
{
    [RoutePrefix("api/graph")]
    public class MyApiController : ApiController
    {
        // GET: Weather
        [Route("")]
        [HttpGet]
        public IEnumerable<WeatherView> Get()
        {
            List<Weather> weather = new List<Weather>();

            using (var context = new WeatherDbContext())
            {
                weather = context.Weather.OrderByDescending(x => x.DATETIME).ToList();
            }

            List<WeatherView> weatherViews = new List<WeatherView>();

            foreach (var item in weather)
            {
                weatherViews.Add(new WeatherView(item));
            }
            return weatherViews;
        }
    }
}