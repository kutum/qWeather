namespace qWeather.Models
{
    public class WeatherView
    {
        public int id { get; set; }
        public string datetime { get; set; }
        public float outsideTemp { get; set; }
        public float insideTemp { get; set; }

        public WeatherView() { }

        public WeatherView(Weather weather)
        {
            id = weather.Id;
            datetime = weather.DATETIME.ToString("yyyy-MM-dd HH:mm:ss");
            outsideTemp = weather.VAL1;
            insideTemp = weather.VAL2;
        }
    }
}