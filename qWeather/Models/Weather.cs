using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

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
    }
}