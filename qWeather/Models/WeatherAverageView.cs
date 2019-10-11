namespace qWeather.Models
{
    public class WeatherAverageView
    {
        public string datetimefrom { get; set; }
        public string datetimeend { get; set; }
        public float outsideTemp { get; set; }
        public float insideTemp { get; set; }

        public WeatherAverageView() { }
    }
}