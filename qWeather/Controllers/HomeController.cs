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
            return View();
        }
    }
}