using qWeather.Context;
using qWeather.Models;
using qWeather.Models.ESP8266;
using System;
using System.Configuration;
using System.ServiceProcess;
using System.Timers;

namespace espService
{
    /// <summary>
    /// Сервис получения и записи данных с датчиков
    /// </summary>
    public partial class EspService : ServiceBase, IEspService
    {
        /// <summary>
        /// Логгирование
        /// </summary>
        private readonly Logging logging;
        /// <summary>
        /// Класс данных с контроллера
        /// </summary>
        private ESPData ESPData;
        /// <summary>
        /// Сетевой адрес контроллера
        /// </summary>
        private readonly Uri ESPurl;
        /// <summary>
        /// Таймер выполнения сервиса
        /// </summary>
        private readonly Timer timer;
        /// <summary>
        /// Контекст БД
        /// </summary>
        private readonly WeatherDbContext context;
        /// <summary>
        /// Методы работы с HTTP запросами
        /// </summary>
        private readonly EspServiceHttp espServiceHttp;

        /// <summary>
        /// Конструктор для заполнения данных сервиса получения данных с контроллера
        /// </summary>
        public EspService()
        {
            InitializeComponent();
            logging = new Logging();
            ESPData = new ESPData();
            ESPurl = new Uri(ConfigurationManager.AppSettings["ESP8266url"]);
            timer = new Timer { Interval = TimeSpan.FromMinutes(Convert.ToDouble(ConfigurationManager.AppSettings["TimerInterval"].Replace(".", ","))).TotalMilliseconds };
            context = new WeatherDbContext();
            espServiceHttp = new EspServiceHttp();
        }

        /// <summary>
        /// Статус доступности контроллера
        /// </summary>
        /// <returns>Статус</returns>
        public string Status()
        {
            return espServiceHttp.Status(ESPurl, logging);
        }

        /// <summary>
        /// Получение данных с контроллера
        /// </summary>
        /// <returns>Данные контроллера</returns>
        public ESPData Espdata()
        {
            return espServiceHttp.Espdata(ESPurl, logging, ESPData);
        }

        /// <summary>
        /// Запуск сервиса
        /// </summary>
        /// <param name="args">Параметры запуска</param>
        protected override void OnStart(string[] args)
        {
            logging.WriteLog("Service started");
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTimeAsync);
            timer.Enabled = true;
        }

        /// <summary>
        /// Выполнение по пройденному времени
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private async void OnElapsedTimeAsync(object source, ElapsedEventArgs e)
        {
            try
            {
                ESPData = await ESPData.GetAsync(ESPurl);

                var Message = new string[]
                {
                    "get from controller by IP: " + ESPurl,
                    "name:" + ESPData.name,
                    "T_OUT:" + ESPData.variables.T_OUT.ToString(),
                    "T_IN:" + ESPData.variables.T_IN.ToString(),
                    "Humidity:" + ESPData.variables.Humidity.ToString()
                };

                logging.WriteLog(Message);

                InsertIntoDB(ESPData);
            }
            catch (Exception ex)
            {
                logging.WriteLog(ex);
            }
        }

        /// <summary>
        /// Вставка данных с контроллера в БД
        /// </summary>
        /// <param name="espdata">Данные с контроллера</param>
        private async void InsertIntoDB(ESPData espdata)
        {
            try
            {
                context.Weather.Add(new Weather
                {
                    DATETIME = DateTime.Now,
                    VAL1 = espdata.variables.T_OUT,
                    VAL2 = espdata.variables.T_IN,
                    HUMIDITY = espdata.variables.Humidity
                });

                await context.SaveChangesAsync();

                logging.WriteLog("Successfull insert into DB: " + context.Database.Connection.Database);
            }
            catch (Exception ex)
            {
                logging.WriteLog(ex);
                throw new Exception(ex.Message);
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
