﻿using System;

namespace qWeather.Models
{
    public static class Extensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }
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

        public static float? Round(float? sum, int count, int round)
        {
            return (float?)Math.Round((decimal)sum / count, round);
        }

    }
}