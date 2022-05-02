using System;

namespace qWeather.Models.ESP8266
{
    /// <summary>
    /// Класс данных с контроллера
    /// </summary>
    public class ESPData
    {
        public DateTime DateTime { get; set; }
        /// <summary>
        /// Температура внутренняя
        /// </summary>
        public float? T_IN { get; set; }
        /// <summary>
        /// Температура наружняя
        /// </summary>
        public float? T_OUT { get; set; }
        /// <summary>
        /// Наружняя влажность
        /// </summary>
        public float? Humidity { get; set; }
    }
}