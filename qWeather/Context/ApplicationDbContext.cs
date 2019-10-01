using qWeather.Models;
using System.Data.Entity;

namespace qWeather.Context
{
    [DbConfigurationType(typeof(MySql.Data.EntityFramework.MySqlEFConfiguration))]
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