using qWeather.Models;
using System.Data.Entity;

namespace qWeather.Context
{
    /// <summary>
    /// Контекст подключения к БД
    /// </summary>
    public class WeatherDbContext : DbContext
    {
        public WeatherDbContext() : base("DefaultConnection") { }

        public static WeatherDbContext Create()
        {
            return new WeatherDbContext();
        }

        public virtual DbSet<Weather> Weather { get; set; }
    }
}