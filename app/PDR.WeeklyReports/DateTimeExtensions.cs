using System;

namespace PDR.WeeklyReports
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfCurrentWeek(this DateTime dt)
        {
            DayOfWeek startOfWeek = DayOfWeek.Monday;
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime EndOfCurrentWeek(this DateTime dt)
        {
            var result = dt.AddDays(7).AddSeconds(-1);
            return result;
        }

        public static DateTime StartOfPreviousYear(this DateTime dt)
        {
            var previousYear = dt.Year - 1;
            var result = new DateTime(previousYear, 1, 1, 0, 0, 0);
            return result;
        }

        public static DateTime StartOfCurrentYear(this DateTime dt)
        {
            var result = new DateTime(dt.Year, 1, 1, 0, 0, 0);
            return result;
        }

        public static DateTime EndOfPreviousYear(this DateTime dt)
        {
            var previousYear = dt.AddYears(-1);
            var result = new DateTime(previousYear.Year, 12, DateTime.DaysInMonth(previousYear.Year, 12), 23, 59, 59);
            return result;
        }

        public static DateTime StartOfPreviousMonth(this DateTime dt)
        {
            var previousMonth = dt.AddMonths(-1);
            var result = new DateTime(previousMonth.Year, previousMonth.Month, 1, 0, 0, 0);
            return result;
        }

        public static DateTime EndOfPreviousMonth(this DateTime dt)
        {
            var previousMonth = dt.AddMonths(-1);
            DateTime endOfMonth = new DateTime(previousMonth.Year, previousMonth.Month, DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month), 23, 59, 59);
            return endOfMonth;
        }
    }
}
