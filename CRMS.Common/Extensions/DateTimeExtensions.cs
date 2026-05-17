using System;
using System.Collections.Generic;

namespace CRMS.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToLongDateString(this DateTime date)
        {
            return date.ToString("dddd, dd MMMM yyyy");
        }

        public static string ToShortDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm");
        }

        public static bool IsToday(this DateTime date)
        {
            return date.Date == DateTime.Today;
        }

        public static bool IsThisMonth(this DateTime date)
        {
            return date.Year == DateTime.Now.Year && date.Month == DateTime.Now.Month;
        }

        public static bool IsThisYear(this DateTime date)
        {
            return date.Year == DateTime.Now.Year;
        }

        public static bool IsPastDate(this DateTime date)
        {
            return date.Date < DateTime.Today;
        }

        public static bool IsFutureDate(this DateTime date)
        {
            return date.Date > DateTime.Today;
        }

        public static int GetDaysUntil(this DateTime date)
        {
            return (int)(date.Date - DateTime.Today).TotalDays;
        }
    }
}
