using System.Collections.Generic;

namespace qWeather.Models
{
    /// <summary>
    /// Класс для представления данных о средней температуре
    /// </summary>
    public class WeatherAverageView
    {
        /// <summary>
        /// Средняя наружняя температура
        /// </summary>
        public float? outsideTemp { get; set; }
        /// <summary>
        /// Средняя внутренняя температура
        /// </summary>
        public float? insideTemp { get; set; }
        /// <summary>
        /// Средняя влажность
        /// </summary>
        public float? humidity { get; set; }

        public WeatherAverageView (List <Weather> weathers)
        {
            float? outsideTempSum = 0.0f;
            float? insideTempSum = 0.0f;
            float? humiditySum = 0.0f;

            foreach (Weather item in weathers)
            {
                outsideTempSum += item.VAL2;
                insideTempSum += item.VAL1;
                humiditySum += item.HUMIDITY;
            }

            outsideTemp = weathers.Count > 0 ? outsideTempSum / weathers.Count : 0.0f;
            insideTemp = weathers.Count > 0 ? insideTempSum / weathers.Count : 0.0f;
            humidity =  weathers.Count > 0 ? humiditySum / weathers.Count : 0.0f;
        }

    }
}