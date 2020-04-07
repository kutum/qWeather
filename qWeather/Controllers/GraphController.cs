using Newtonsoft.Json;
using qWeather.Context;
using qWeather.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using HttpGetAttribute = System.Web.Mvc.HttpGetAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using RoutePrefixAttribute = System.Web.Http.RoutePrefixAttribute;

namespace qWeather.Controllers
{
    [RoutePrefix("api/graph")]
    public class MyApiController : ApiController
    {
        public List<Weather> weather = new List<Weather>();
        public List<WeatherView> weatherViews = new List<WeatherView>();

        // GET: Weather
        [Route("")]
        [HttpGet]
        public IEnumerable<WeatherView> Get()
        {
            using (var context = new WeatherDbContext())
            {
                weather = context.Weather.OrderByDescending(x => x.DATETIME).ToList();
            }

            foreach (var item in weather)
            {
                weatherViews.Add(new WeatherView(item));
            }
            return weatherViews;
        }

        [Route("")]
        [HttpGet]
        public IEnumerable<WeatherView> Get([FromUri]DateTime start, [FromUri]DateTime end)
        {
            using (var context = new WeatherDbContext())
            {
                weather = context.Weather.Where(x => x.DATETIME >= start && x.DATETIME <= end).OrderByDescending(x => x.DATETIME).ToList();
            }

            foreach (var item in weather)
            {
                weatherViews.Add(new WeatherView(item));
            }
            return weatherViews;
        }

        [Route("last")]
        [HttpGet]
        public IEnumerable<WeatherView> GetLast()
        {
            using (var context = new WeatherDbContext())
            {
                weather = context.Weather.Where(x => x.DATETIME == context.Weather.Max(y => y.DATETIME)).OrderByDescending(x => x.DATETIME).ToList();
            }

            foreach (var item in weather)
            {
                weatherViews.Add(new WeatherView(item));
            }
            return weatherViews;
        }
        [Route("average")]
        [HttpGet]
        public WeatherAverageView GetAverage([FromUri]DateTime start, [FromUri]DateTime end)
        {

            using (var context = new WeatherDbContext())
            {
                weather = context.Weather.Where(x => x.DATETIME >= start && x.DATETIME <= end).ToList();
            }

            float insideSum = 0.0f;
            float outsideSum = 0.0f;

            foreach (var item in weather)
            {
                insideSum += item.VAL2;
                outsideSum += item.VAL1;
            }

            WeatherAverageView weatherAverageView = new WeatherAverageView() {
                datetimefrom = start.ToString("yyyy-MM-dd HH:mm:ss"),
                datetimeend = end.ToString("yyyy-MM-dd HH:mm:ss"),
                insideTemp = insideSum / weather.Count,
                outsideTemp = outsideSum / weather.Count
            };

            return weatherAverageView;
        }
        [Route("T_IN")]
        [HttpGet]
        public async Task<TData> GetT_INAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://192.168.1.89/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res = await client.GetAsync("T_IN");

                var T_InResponse = Res.Content.ReadAsStringAsync().Result;

                TData data = new TData();

                return JsonConvert.DeserializeObject<TData>(T_InResponse);
            }
        }

        public class TData
        {
            public string T_IN { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string hardware { get; set; }
            public string connected { get; set; }
        }
    }
}