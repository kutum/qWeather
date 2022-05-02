using System;
using System.Threading.Tasks;

namespace qWeather.Models.ESP8266.Interfaces
{
    public interface IESPMethods
    {
        /// <summary>
        /// Получение данных с датчика асинхронно
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        ESPData Get(Uri URL);

        string GetString(Uri URL);

        ESPData GetJson(Uri URL);
    }
}
