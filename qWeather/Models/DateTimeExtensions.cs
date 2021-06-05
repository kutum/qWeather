using System;

namespace qWeather.Models
{
    /// <summary>
    /// Хелпер для дат
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Начало недели
        /// </summary>
        /// <param name="dt">Дата</param>
        /// <param name="startOfWeek">День начала недели</param>
        /// <returns>Дата начала недели</returns>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// Конец недели
        /// </summary>
        /// <param name="dt">Дата</param>
        /// <param name="endOfWeek">День конца недели</param>
        /// <returns>Дата конца недели</returns>
        public static DateTime EndOfWeek(this DateTime dt, DayOfWeek endOfWeek = DayOfWeek.Sunday)
        {
            if (dt.DayOfWeek == endOfWeek)
            {
                return dt.Date.Date.AddDays(1).AddMilliseconds(-1);
            }
            else
            {
                var diff = dt.DayOfWeek - endOfWeek;
                return dt.AddDays(7 - diff).Date.AddDays(1).AddMilliseconds(-1);
            }
        }
    }
}