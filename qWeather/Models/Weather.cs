using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace qWeather.Models
{
    [Table ("WEATHER")]
    public class Weather
    {
        [Key]
        public int Id { get; set; }
        public DateTime DATETIME { get; set; }
        public float VAL1 { get; set; }
        public float VAL2 { get; set; }
        public float? HUMIDITY { get; set; }
    }
}