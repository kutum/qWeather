namespace qWeather.Models.ESP8266
{
    /// <summary>
    /// Показания датчиков контроллера
    /// </summary>
    public class ESPSensors
    {
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