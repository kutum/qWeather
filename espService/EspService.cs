using espService.Interfaces;
using qWeather.Context;
using qWeather.Models;
using qWeather.Models.ESP8266;
using qWeather.Models.ESP8266.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;

namespace espService
{
    /// <summary>
    /// Сервис получения и записи данных с датчиков
    /// </summary>
    public partial class EspService : ServiceBase, IEspService
    {

        private IESPMethods espMethods;
        /// <summary>
        /// Методы работы с HTTP запросами
        /// </summary>
        private IEspServiceHttp espServiceHttp;
        /// <summary>
        /// Логгирование
        /// </summary>
        private readonly ILogging logging;
        /// <summary>
        /// Сетевой адрес контроллера
        /// </summary>
        private readonly Uri ESPurl;
        /// <summary>
        /// Таймер выполнения сервиса
        /// </summary>
        private readonly Timer timer;

        /// <summary>
        /// Конструктор для заполнения данных сервиса получения данных с контроллера
        /// </summary>
        public EspService()
        {
            InitializeComponent();
            logging = new Logging();
            ESPurl = new Uri(ConfigurationManager.AppSettings["ESP8266url"]);
            timer = new Timer { Interval = TimeSpan.FromMinutes(Convert.ToDouble(ConfigurationManager.AppSettings["TimerInterval"].Replace(".", ","))).TotalMilliseconds };
            espMethods = new ESPMethods();
            espServiceHttp = new EspServiceHttp(logging);
        }

        /// <summary>
        /// Статус доступности контроллера
        /// </summary>
        /// <returns>Статус</returns>
        public string Status()
        {
            return espServiceHttp.Status(ESPurl);
        }

        public ESPData Now()
        {
            return espMethods.Get(ESPurl);
        }

        /// <summary>
        /// Запуск сервиса
        /// </summary>
        /// <param name="args">Параметры запуска</param>
        protected override void OnStart(string[] args)
        {
            logging.WriteLog("Service started");
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Enabled = true;
        }

        /// <summary>
        /// Выполнение по пройденному времени
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            try
            {
                var files = espMethods.GetString(new Uri(ESPurl.AbsoluteUri + "files"));

                if (string.IsNullOrEmpty(files))
                    throw new Exception("Files not found");

                var fileNames = files.Split(',').ToList();

                List<Weather> espdatas = new List<Weather>();
                foreach (var fileName in fileNames)
                {
                    var data = espMethods.GetString(new Uri(ESPurl.AbsoluteUri + "fdata?date=" + fileName));

                    if(string.IsNullOrEmpty(data))
                    {
                        logging.WriteLog("Trying to take data from " + fileName + " but its empty!");
                        return;
                    }

                    foreach (var row in data.Split(';').Where(x => x.Length > 0))
                    {
                        var items = row.Split(',');

                        var Date = DateTime.ParseExact(fileName + " " + items[0], "yyyy-MM-dd H:mm:ss", CultureInfo.InvariantCulture);
                        espdatas.Add(new Weather
                        {
                            DATETIME = Date,
                            VAL2 = float.Parse(items[1].Replace('.', ',')),
                            VAL1 = float.Parse(items[2].Replace('.', ',')),
                            HUMIDITY = float.Parse(items[3].Replace('.', ','))
                        });
                    }
                }
                try
                {
                    using (var context = new WeatherDbContext())
                    {
                        var lastDate = context.Weather.Max(x => x.DATETIME);
                        var espDatasInsert = espdatas.Where(x => x.DATETIME > lastDate);

                        context.Weather.AddRange(espDatasInsert);
                        var count = context.SaveChanges();
                        logging.WriteLog("Inserted " + count + " items in " + context.Database.Connection.Database);
                    }
                }
                catch (Exception ex)
                {
                    logging.WriteLog(ex);
                    logging.WriteLog(espdatas);
                    throw new Exception("Error while insert in database", ex);
                }

                foreach(var fileName in fileNames)
                    espMethods.GetString(new Uri(ESPurl.AbsoluteUri + "delete?name=" + fileName));

            }
            catch (Exception ex)
            {
                logging.WriteLog(ex);
            }
        }

        /// <summary>
        /// Остановка сервиса
        /// </summary>
        protected override void OnStop()
        {
            timer.Stop();
            logging.WriteLog("Service stopped");
        }

        /// <summary>
        /// Сервисный режим
        /// </summary>
        public void OnDebug()
        {
            OnStart(null);
        }
    }
}
