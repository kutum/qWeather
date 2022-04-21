namespace qWeather.Models.ESP8266
{
    /// <summary>
    /// Класс данных с контроллера
    /// </summary>
    public class ESPData
    {
        /// <summary>
        /// Датчики
        /// </summary>
        public ESPSensors variables { get; set; }
        /// <summary>
        /// ID контроллера
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// Название контроллера
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Наимеование чипа контроллера
        /// </summary>
        public string hardware { get; set; }
        /// <summary>
        /// Статус подключения
        /// </summary>
        public string connected { get; set; }
    }
}