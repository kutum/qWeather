using qWeather.Context;
using qWeather.Models;
using qWeather.Models.ESP8266;
using qWeather.SignalR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;

namespace qWeather.Controllers
{
    /// <summary>
    /// Контроллер получения данных для страницы с датчиков
    /// </summary>
    [RoutePrefix("api")]
    public class WebController : ApiController
    {
        /// <summary>
        /// Контекст подключения к БД
        /// </summary>
        private WeatherDbContext context = new WeatherDbContext();

        /// <summary>
        /// Логгирование
        /// </summary>
        private Logging logging = new Logging();

        /// <summary>
        /// Список данных с датчиков
        /// </summary>
        private List<Weather> weathers = new List<Weather>();

        /// <summary>
        /// 
        /// </summary>
        private SignalRepository signalRepository = new SignalRepository();

        /// <summary>
        /// Получить все записи отсортированные по времени по возрастанию
        /// </summary>
        /// <returns>Список показаний с датчиков</returns>
        [Route("")]
        [HttpGet()]
        public IEnumerable<Weather> Get()
        {
            return context.Weather.OrderBy(x => x.DATETIME);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("getmessages")]
        [HttpGet()]
        public string GetMessages()
        {
            return signalRepository.GetAllMessages(context, logging);
        }

        /// <summary>
        /// Получить записи в промежутке по возрастанию
        /// </summary>
        /// <param name="start">Начало периода</param>
        /// <param name="end">Конец периода</param>
        /// <returns>Список показаний с датчиков</returns>
        [Route("")]
        [HttpGet()]
        public IEnumerable<Weather> Get([FromUri]DateTime start, [FromUri]DateTime end)
        {
            return context.Weather.Where(x => x.DATETIME >= start && x.DATETIME <= end).OrderBy(x => x.DATETIME);
        }

        /// <summary>
        /// Получить последнее показание в БД с датчика
        /// </summary>
        /// <returns>Показания с датчиков</returns>
        [Route("last")]
        [HttpGet()]
        public Task<Weather> GetLast()
        {
            return context.Weather.OrderByDescending(x => x.DATETIME).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Получить показания датчиков из сервиса
        /// </summary>
        /// <returns>Показания с датчиков</returns>
        [Route("now")]
        [HttpGet]
        public async Task<Weather> Now()
        {
            try
            {
                ESPData ESPData = new ESPData();
                ESPData = await ESPData.GetAsync(new Uri(WebConfigurationManager.AppSettings["espServiceUrl"]));

                return new Weather
                {
                    Id = ESPData.id,
                    DATETIME = DateTime.Now,
                    VAL1 = ESPData.variables.T_OUT,
                    VAL2 = ESPData.variables.T_IN,
                    HUMIDITY = ESPData.variables.Humidity
                };
            }
            catch (Exception ex)
            {
                logging.WriteLog(ex);
                throw new Exception(ex.Message + " " + ex.InnerException);
            }
        }

        /// <summary>
        /// Получить среднее значение показаний за период
        /// </summary>
        /// <param name="start">Начало периода</param>
        /// <param name="end">Конец периода</param>
        /// <returns>Показания с датчиков</returns>
        [Route("average")]
        [HttpGet()]
        public async Task<WeatherAverageView> GetAverage([FromUri]DateTime start, [FromUri]DateTime end)
        {            
            return new WeatherAverageView(await context.Weather.Where(x => x.DATETIME >= start && x.DATETIME <= end).ToListAsync());
        }

        /// <summary>
        /// Получить среднее значение показаний за день
        /// </summary>
        /// <param name="date">День</param>
        /// <returns>Показания с датчиков</returns>
        [Route("averagebyday")]
        [HttpGet()]
        public async Task<WeatherAverageView> GetAverageByDay([FromUri] DateTime date)
        {
            var start = date.AddHours(-24);
            return new WeatherAverageView(await context.Weather.Where(x => x.DATETIME >= start && x.DATETIME <= date).ToListAsync());
        }

        /// <summary>
        /// Получить среднее значение показаний за неделю
        /// </summary>
        /// <param name="date">День недели</param>
        /// <returns>Показания с датчиков</returns>
        [Route("averagebyweek")]
        [HttpGet]
        public async Task<WeatherAverageView> GetAverageByWeek([FromUri] DateTime date)
        {
            var start = date.StartOfWeek();
            var end = date.EndOfWeek();

            return new WeatherAverageView(await context.Weather.Where(x => x.DATETIME >= start && x.DATETIME <= end).ToListAsync());
        }

        /// <summary>
        /// Получить среднее значение показаний за месяц
        /// </summary>
        /// <param name="date">День месяца</param>
        /// <returns>Показания с датчиков</returns>
        [Route("averagebymonth")]
        [HttpGet()]
        public async Task<WeatherAverageView> GetAverageByMonth([FromUri] DateTime date)
        {
            var start = new DateTime(date.Year, date.Month, 1);
            return new WeatherAverageView(await context.Weather.Where(x => x.DATETIME >= start && x.DATETIME <= date).ToListAsync());
        }

        /// <summary>
        /// Получить среднее значение показаний за год
        /// </summary>
        /// <param name="date">День года</param>
        /// <returns>Показания с датчиков</returns>
        [Route("averagebyyear")]
        [HttpGet()]
        public async Task<WeatherAverageView> GetAverageByYear([FromUri] DateTime date)
        {
            var start = new DateTime(date.Year, 1, 1);
            return new WeatherAverageView(await context.Weather.Where(x => x.DATETIME >= start && x.DATETIME <= date).ToListAsync());
        }

        /// <summary>
        /// Получить показания группированные по часам за день
        /// </summary>
        /// <param name="date">Текущий день</param>
        /// <returns>Список показаний с датчиков</returns>
        [Route("day")]
        [HttpGet()]
        public async Task<IEnumerable<Weather>> GetByDay([FromUri] DateTime date)
        {
            var start = date.AddDays(-1);
            var weatherGroups = await context.Weather.Where(x => x.DATETIME>= start &&
                                                                 x.DATETIME <= date)
                                                                    .GroupBy(x => x.DATETIME.Hour).ToListAsync();
            foreach (var item in weatherGroups)
            {
                weathers.AddRange(weatherGroups.Where(x => x.Key == item.Key)
                    .Select(itemByHour =>
                    new Weather
                    {
                        DATETIME = new DateTime(date.Year, date.Month, itemByHour.FirstOrDefault().DATETIME.Day, item.Key, 0, 0),
                        VAL1 = itemByHour.Sum(x => x.VAL1) / itemByHour.Count(),
                        VAL2 = itemByHour.Sum(x => x.VAL2) / itemByHour.Count(),
                        HUMIDITY = itemByHour.Sum(x => x.HUMIDITY) / itemByHour.Count()

                    }));
            }

            return weathers.OrderBy(x=>x.DATETIME);
        }

        /// <summary>
        /// Получить показания группированные по дням за неделю
        /// </summary>
        /// <param name="date">День недели</param>
        /// <returns>Список показаний с датчиков</returns>
        [Route("week")]
        [HttpGet()]
        public async Task<IEnumerable<Weather>> GetByWeek([FromUri] DateTime date)
        {
            var start = date.StartOfWeek();
            var end = date.EndOfWeek();

            var weatherGroups = await context.Weather.Where(x => x.DATETIME >= start && 
                                                                 x.DATETIME <= end)
                                                                .GroupBy(x=>x.DATETIME.Day).ToListAsync();
            foreach (var item in weatherGroups)
            {
                weathers.AddRange(weatherGroups.Where(x => x.Key == item.Key)
                    .Select(itemByDay => new Weather
                    {
                        DATETIME = new DateTime(date.Year, date.Month, item.Key),
                        VAL1 = itemByDay.Sum(x=>x.VAL1) / itemByDay.Count(),
                        VAL2 = itemByDay.Sum(x=>x.VAL2) / itemByDay.Count(),
                        HUMIDITY = itemByDay.Sum(x=>x.HUMIDITY) / itemByDay.Count()
                    }));
            }
            
            return weathers.OrderBy(x=>x.DATETIME);
        }

        /// <summary>
        /// Получить показания группированные по дням за месяц
        /// </summary>
        /// <param name="date">День месяца</param>
        /// <returns>Список показаний с датчиков</returns>
        [Route("month")]
        [HttpGet()]
        public async Task<IEnumerable<Weather>> GetByMonth([FromUri] DateTime date)
        {
            var start = new DateTime(date.Year, date.Month, 1);
            
            var weatherGroups = await context.Weather.Where(x => x.DATETIME >= start && 
                                                                 x.DATETIME <= date)
                                                                    .GroupBy(x => x.DATETIME.Day).ToListAsync();
            foreach(var item in weatherGroups)
            {
                weathers.AddRange(weatherGroups.Where(x => x.Key == item.Key)
                    .Select(itemByDay => new Weather
                    {
                        DATETIME = new DateTime(date.Year, date.Month, item.Key),
                        VAL1 = itemByDay.Sum(x => x.VAL1) / itemByDay.Count(),
                        VAL2 = itemByDay.Sum(x => x.VAL2) / itemByDay.Count(),
                        HUMIDITY = itemByDay.Sum(x => x.HUMIDITY) / itemByDay.Count()
                    }));
            }

            return weathers.OrderBy(x => x.DATETIME);
        }

        /// <summary>
        /// Получить показания группированные по месяцам за год
        /// </summary>
        /// <param name="date">День года</param>
        /// <returns>Список показаний с датчиков</returns>
        [Route("year")]
        [HttpGet()]
        public async Task<IEnumerable<Weather>> GetByYear([FromUri] DateTime date)
        {
            var start = new DateTime(date.Year, 1, 1);

            var weatherGroups = await context.Weather.Where(x => x.DATETIME >= start &&
                                                               x.DATETIME <= date)
                                                                    .GroupBy(x => x.DATETIME.Month).ToListAsync();
            foreach(var item in weatherGroups)
            {
                weathers.AddRange(weatherGroups.Where(x => x.Key == item.Key)
                 .Select(itemByMonth => new Weather
                 {
                     DATETIME = new DateTime(date.Year, item.Key, 1),
                     VAL1 = itemByMonth.Sum(x => x.VAL1) / itemByMonth.Count(),
                     VAL2 = itemByMonth.Sum(x => x.VAL2) / itemByMonth.Count(),
                     HUMIDITY = itemByMonth.Sum(x => x.HUMIDITY) / itemByMonth.Count()
                 }));
            }

            return weathers.OrderBy(x => x.DATETIME);
        }


        /// <summary>
        /// Получить все показания группированные по месяцам 
        /// </summary>
        /// <returns>Список показаний с датчиков</returns>
        [Route("allyears")]
        [HttpGet()]
        public IEnumerable<Weather> GetByYears()
        {
            var weatherGroups = context.Weather.GroupBy(g => new { g.DATETIME.Year, g.DATETIME.Month }).ToList();

            foreach (var item in weatherGroups)
            {
                weathers.AddRange(weatherGroups.Where(x => x.Key == item.Key)
                    .Select(itemByMonth => new Weather
                    {
                        DATETIME = new DateTime(item.Key.Year, item.Key.Month, 1),
                        VAL1 = itemByMonth.Sum(x => x.VAL1) / itemByMonth.Count(),
                        VAL2 = itemByMonth.Sum(x => x.VAL2) / itemByMonth.Count(),
                        HUMIDITY = itemByMonth.Sum(x => x.HUMIDITY) / itemByMonth.Count()
                    }));
            }

            return weathers.OrderBy(x => x.DATETIME);
        }
    }
}