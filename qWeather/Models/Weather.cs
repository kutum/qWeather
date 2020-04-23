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
        public float? HUMIDITY { get; set; }

        /// <summary>
        /// Форматированный вид даты и времени
        /// </summary>
        [NotMapped]
        public string DateTimeFormatted
        {
            get => DATETIME.ToString("yyyy-MM-dd HH:mm:s");
            set => DateTimeFormatted = this.DateTimeFormatted;
        }

        /// <summary>
        /// Форматированный вид даты и времени "по русски"
        /// </summary>
        [NotMapped]
        public string DateTimeFormattedRus
        {
            get => DATETIME.ToString("dd.MM.yyyy HH:mm");
            set => DateTimeFormattedRus = this.DateTimeFormattedRus;
        }
    }
}