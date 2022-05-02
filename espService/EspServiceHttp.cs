using espService.Interfaces;
using qWeather.Models;
using System;
using System.Net.NetworkInformation;

namespace espService
{
    /// <summary>
    /// Обработка HTTP запросов к сервису
    /// </summary>
    public class EspServiceHttp : IEspServiceHttp
    {
        private ILogging logging;

        /// <summary>
        /// Пинг проверка доступности контроллера
        /// </summary>
        private readonly Ping ping;

        public EspServiceHttp (ILogging _logging)
        {
            logging = _logging;
            ping = new Ping();
        }

        /// <summary>
        /// Статус доступности контроллера
        /// </summary>
        /// <returns>Статус</returns>
        public string Status(Uri uri)
        {
            logging.WriteLog("Status response");
            var pingable = false;

            try
            {
                PingReply reply = ping.Send(uri.Host);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException ex)
            {
                logging.WriteLog(ex);
                throw new Exception(ex.Message);
            }

            return pingable ? "Online" : "Offline";
        }
    }
}
