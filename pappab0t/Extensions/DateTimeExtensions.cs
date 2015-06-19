using System;
using System.Globalization;

namespace pappab0t.Extensions
{
    public static class DateTimeExtensions
    {
        public static int Iso8601Week(this DateTime time)
        {
            //http://stackoverflow.com/questions/11154673/get-the-correct-week-number-of-a-given-date

            var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}