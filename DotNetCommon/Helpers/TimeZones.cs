namespace DotNetCommon.Helpers
{
    public static class TimeZones
    {
        public static TimeSpan GetCentralTimeOffset()
        {
            TimeZoneInfo item = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            TimeSpan offset = item.GetUtcOffset(DateTime.UtcNow);
            return offset;
        }

        public static TimeSpan GetCentralTimeOffset(DateTime dateTime)
        {
            TimeZoneInfo item = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            TimeSpan offset = item.GetUtcOffset(dateTime);
            return offset;
        }

        public static TimeSpan GetUtcOffset()
        {
            TimeZoneInfo utc = TimeZoneInfo.FindSystemTimeZoneById("UTC");
            TimeSpan offset = utc.GetUtcOffset(DateTime.UtcNow);
            return offset;
        }
    }
}