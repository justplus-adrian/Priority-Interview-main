using System.Globalization;

namespace InterviewApi.Helpers
{

    public static class CalendarExtensions
    {
        public static int GetWeekOfMonth(this Calendar calendar, DateTime date)
        {
            var first = new DateTime(date.Year, date.Month, 1);
            return calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                    - calendar.GetWeekOfYear(first, CalendarWeekRule.FirstDay, DayOfWeek.Monday) + 1;
        }
    }
}
