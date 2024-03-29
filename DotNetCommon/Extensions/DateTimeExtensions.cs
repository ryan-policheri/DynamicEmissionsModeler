﻿namespace DotNetCommon.Extensions
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

        public static string ToStringWithNoOffset(this DateTimeOffset dateTime, string format = "yyyy-MM-ddTHH:mm:ssZ")
        {
            dateTime = dateTime.ToUniversalTime();
            return dateTime.ToString(format);
        }

        public static bool HourMatches(this DateTimeOffset dateTimeOffset1, DateTimeOffset dateTimeOffset2)
        {
            return (dateTimeOffset1.UtcDateTime.Date == dateTimeOffset2.UtcDateTime.Date &&
                    dateTimeOffset1.UtcDateTime.Hour == dateTimeOffset2.UtcDateTime.Hour);
        }

        public static ICollection<DateTimeOffset> EnumerateSecondsUntil(this DateTimeOffset startDateTime, DateTimeOffset endDateTime)
        {
            return EnumerateTimePoints(startDateTime, endDateTime, (inPoint) => inPoint.AddSeconds(1));
        }

        public static ICollection<DateTimeOffset> EnumerateMinutesUntil(this DateTimeOffset startDateTime, DateTimeOffset endDateTime)
        {
            return EnumerateTimePoints(startDateTime, endDateTime, (inPoint) => inPoint.AddMinutes(1));
        }

        public static ICollection<DateTimeOffset> EnumerateHoursUntil(this DateTimeOffset startDateTime, DateTimeOffset endDateTime)
        {
            return EnumerateTimePoints(startDateTime, endDateTime, (inPoint) => inPoint.AddHours(1));
        }

        public static ICollection<DateTimeOffset> EnumerateDaysUntil(this DateTimeOffset startDateTime, DateTimeOffset endDateTime)
        {
            return EnumerateTimePoints(startDateTime, endDateTime, (inPoint) => inPoint.AddDays(1).Date);
        }

        private static ICollection<DateTimeOffset> EnumerateTimePoints(DateTimeOffset startTime, DateTimeOffset endTime, Func<DateTimeOffset, DateTimeOffset> incrementFunc)
        {
            if (startTime > endTime) throw new ArgumentException("startDateTime must be less than or equal to endDateTime");
            ICollection<DateTimeOffset> timePoints = new List<DateTimeOffset>();
            DateTimeOffset dateIterator = startTime;
            while (dateIterator <= endTime)
            {
                timePoints.Add(dateIterator);
                dateIterator = incrementFunc(dateIterator);
            }
            return timePoints;
        }

        public static bool AllHoursMatch(this IEnumerable<DateTimeOffset> dataSet1, IEnumerable<DateTimeOffset> dataSet2)
        {
            if (dataSet1.Count() != dataSet2.Count()) return false;

            var orderedSet1 = dataSet1.OrderBy(x => x);
            var orderedSet2 = dataSet2.OrderBy(x => x);

            for(int i = 0; i < orderedSet1.Count(); i++)
            {
                DateTimeOffset set1TimeStamp =  orderedSet1.ElementAt(i);
                DateTimeOffset set2TimeStamp =  orderedSet2.ElementAt(i);

                if(!set1TimeStamp.HourMatches(set2TimeStamp)) return false;
            }

            return true;
        }
    }
}