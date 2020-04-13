using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace qWeather.Models
{
    [Table ("WEATHER")]
    public class Weather
    {
        /// <summary>
        /// ID Записи
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Дата и время замера
        /// </summary>
        public DateTime DATETIME { get; set; }
        /// <summary>
        /// Наружняя температура
        /// </summary>
        public float? VAL1 { get; set; }
        /// <summary>
        /// Комнатная температура
        /// </summary>
        public float? VAL2 { get; set; }
        /// <summary>
        /// Влажность
        /// </summary>
        public int? HUMIDITY { get; set; }

        [NotMapped]
        public string DateTimeFormatted
        {
            get => DATETIME.ToString("yyyy-MM-dd HH:mm:s");
            set => DateTimeFormatted = DATETIME.ToString("yyyy-MM-dd HH:mm:s");
        }
    }
}