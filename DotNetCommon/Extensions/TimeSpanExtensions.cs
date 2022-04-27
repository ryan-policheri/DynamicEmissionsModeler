namespace DotNetCommon.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string ToHourString(this TimeSpan timeSpan)
        {
            if (timeSpan == null) return null;
            string hours = timeSpan.ToString("hh");
            if (timeSpan.TotalSeconds < 0.0) return "-" + hours;
            else return "+" + hours;
        }
    }
}