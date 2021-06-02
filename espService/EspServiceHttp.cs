using qWeather.Models;
using qWeather.Models.ESP8266;
using System;
using System.Net.NetworkInformation;

namespace espService
{
    /// <summary>
    /// Обработка HTTP запросов к сервису
    /// </summary>
    public class EspServiceHttp
    {
        /// <summary>
        /// Пинг проверка доступности контроллера
        /// </summary>
        private readonly Ping ping = new Ping();

        /// <summary>
        /// Статус доступности контроллера
        /// </summary>
        /// <returns>Статус</returns>
        public string Status(Uri uri, Logging logging)
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

        /// <summary>
        /// Получение данных с контроллера
        /// </summary>
        /// <returns>Данные контроллера</returns>
        public ESPData Espdata(Uri uri, Logging logging, ESPData eSPData)
        {
            logging.WriteLog("Espdata response");

            try
            {
                var Data = eSPData.Get(uri);
                logging.WriteLog(Data);
                return Data;
            }
            catch (Exception ex)
            {
                logging.WriteLog(ex);
                throw new Exception(ex.Message);
            }
        }
    }
}
