namespace khyata.Application.Helpers
{
    public static class DateHelper
    {
        public static DateTime GetEndOfMonth(DateTime date)
        {
            return new DateTime(
                date.Year,
                date.Month,
                DateTime.DaysInMonth(date.Year, date.Month));
        }
    }
}
