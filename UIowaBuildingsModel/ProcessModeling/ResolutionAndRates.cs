﻿using DotNetCommon.Extensions;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class DataResolution
    {
        public const string EverySecond = "Every Second";
        public const string EveryMinute = "Every Minute";
        public const string Hourly = "Hourly";
        public const string Daily = "Daily";

        public static List<string> ToListing()
        {
            return new List<string> { EverySecond, EveryMinute, Hourly, Daily };
        }

        public static ICollection<DateTimeOffset> BuildTimePoints(string resolution, DateTimeOffset startDateTime, DateTimeOffset endDateTime)
        {
            switch (resolution)
            {
                case DataResolution.EverySecond: return startDateTime.EnumerateSecondsUntil(endDateTime);
                case DataResolution.EveryMinute: return startDateTime.EnumerateMinutesUntil(endDateTime);
                case DataResolution.Hourly: return startDateTime.EnumerateHoursUntil(endDateTime);
                case DataResolution.Daily: return startDateTime.EnumerateDaysUntil(endDateTime);
                default: throw new NotImplementedException("render resolution " + resolution + " not  implemented");
            }
        }

        public static DateTimeOffset AddIntervalUnit(string resolution, DateTimeOffset endDateTime)
        {
            switch (resolution)
            {
                case DataResolution.EverySecond: return endDateTime.AddSeconds(1);
                case DataResolution.EveryMinute: return endDateTime.AddMinutes(1);
                case DataResolution.Hourly: return endDateTime.AddHours(1);
                case DataResolution.Daily: return endDateTime.AddDays(1);
                default: throw new NotImplementedException($"need to implement resolution {resolution}");
            }
        }

    }

    public static class DataResolutionExtensions
    {
        public static bool IsSameResolutionAs(this string resolution1, string resolution2)
        {
            return resolution1 == resolution2;
        }

        public static bool IsMoreGranularThan(this string resolution1, string resolution2)
        {
            if (resolution1.IsSameResolutionAs(resolution2)) return false;
            if (resolution2 == DataResolution.Daily) return resolution1.OneOf(new string[] { DataResolution.Hourly, DataResolution.EveryMinute, DataResolution.EverySecond });
            if (resolution2 == DataResolution.Hourly) return resolution1.OneOf(new string[] { DataResolution.EveryMinute, DataResolution.EverySecond });
            if (resolution2 == DataResolution.EveryMinute) return resolution1.OneOf(new string[] { DataResolution.EverySecond });
            if (resolution2 == DataResolution.EverySecond) return false;
            else throw new ArgumentException();
        }

        public static bool IsLessGranularThan(this string resolution1, string resolution2)
        {
            if (resolution1.IsSameResolutionAs(resolution2)) return false;
            else return !resolution1.IsMoreGranularThan(resolution2);
        }

        public static DateTimeOffset TruncateTo(this DateTimeOffset t, string resolution)
        {
            if (resolution == DataResolution.Daily) return new DateTimeOffset(t.Year, t.Month, t.Day, 0, 0, 0, t.Offset);
            if (resolution == DataResolution.Hourly) return new DateTimeOffset(t.Year, t.Month, t.Day, t.Hour, 0, 0, t.Offset);
            if (resolution == DataResolution.EveryMinute) return new DateTimeOffset(t.Year, t.Month, t.Day, t.Hour, t.Minute, 0, t.Offset);
            if (resolution == DataResolution.EverySecond) return new DateTimeOffset(t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second, t.Offset);
            throw new ArgumentException();
        }
    }

    public class DataResolutionPlusVariable : DataResolution
    {
        public const string Variable = "Variable (defer to execution)";

        public static List<string> ToListing()
        {
            var result = DataResolution.ToListing();
            result.Add(Variable);
            return result;
        }
    }

    public class UnitRates
    {
        public const string NoRate = "No Rate";
        public const string PerSecond = "Per Second";
        public const string PerMinute = "Per Minute";
        public const string PerHour = "Per Hour";
        public const string PerDay = "Per Day";
        public const string PerMonth = "Per Year";
        public const string PerQuarter = "Per Quarter";
        public const string PerYear = "Per Year";

        public static List<string> ToListing()
        {
            return new List<string> { PerSecond, PerMinute, PerHour, PerDay, PerMonth, PerQuarter, PerYear, NoRate };
        }
    }
}
