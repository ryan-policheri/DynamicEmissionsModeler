namespace DotNetCommon.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfQuarter(this DateTime dt)
        {
            if (dt.Month >= 1 && dt.Month <= 3) return new DateTime(dt.Year, 1, 1).Date;
            else if (dt.Month >= 4 && dt.Month <= 6) return new DateTime(dt.Year, 4, 1).Date;
            else if (dt.Month >= 7 && dt.Month <= 9) return new DateTime(dt.Year, 7, 1).Date;
            else if (dt.Month >= 10 && dt.Month <= 12) return new DateTime(dt.Year, 10, 1).Date;
            throw new NotSupportedException("DateTime is not following a gregorian calendar");
        }

        public static DateTime EndOfQuarter(this DateTime dt)
        {
            if (dt.Month >= 1 && dt.Month <= 3) return new DateTime(dt.Year, 3, 31).Date;
            else if (dt.Month >= 4 && dt.Month <= 6) return new DateTime(dt.Year, 6, 30).Date;
            else if (dt.Month >= 7 && dt.Month <= 9) return new DateTime(dt.Year, 9, 30).Date;
            else if (dt.Month >= 10 && dt.Month <= 12) return new DateTime(dt.Year, 12, 31).Date;
            throw new NotSupportedException("DateTime is not following a gregorian calendar");
        }

        public static string ToStringWithLocalOffset(this DateTime dateTime, string format = "yyyy-MM-ddTHH:mm:sszzz")
        {
            if (dateTime.Kind == DateTimeKind.Unspecified) throw new ArgumentException("DateTimeKind cannot be unspecified");
            else if (dateTime.Kind == DateTimeKind.Utc) dateTime = dateTime.ToLocalTime();

            TimeSpan baseOffset = TimeZoneInfo.Local.GetUtcOffset(dateTime);
            DateTimeOffset dateTimeWithOffset = new DateTimeOffset(dateTime, baseOffset);
            return dateTimeWithOffset.ToString(format);
        }

        public static string ToStringWithNoOffset(this DateTime dateTime, string format = "yyyy-MM-ddTHH:mm:ssZ")
        {
            if (dateTime.Kind == DateTimeKind.Unspecified) throw new ArgumentException("DateTimeKind cannot be unspecified");
            else if (dateTime.Kind == DateTimeKind.Local) dateTime = dateTime.ToUniversalTime();

            return dateTime.ToString(format);
        }
    }
}