using qWeather.Context;
using qWeather.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace qWeather.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(GraphData());
        }

        public List<Weather> GraphData()
        {
            List<Weather> weather = new List<Weather>();
           
            using (var context = new WeatherDbContext())
            {
                weather = context.Weather.OrderByDescending(x => x.DATETIME).ToList();
            }

            return weather;
        }
    }
}