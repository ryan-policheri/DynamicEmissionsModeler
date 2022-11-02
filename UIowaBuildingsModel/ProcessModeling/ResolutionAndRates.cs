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
    }

    public class DataResolutionPlusVariable : DataResolution
    {
        public const string Variable = "Variable (defer to execution)";

        public static List<string> ToListing()
        {
            var result = ToListing();
            result.Add(Variable);
            return result;
        }
    }

    public class UnitRates
    {
        public const string PerSecond = "Per Second";
        public const string PerMinute = "Per Minute";
        public const string PerHour = "Per Hour";
        public const string PerDay = "Per Day";
        public const string PerMonth = "Per Year";
        public const string PerQuarter = "Per Quarter";
        public const string PerYear = "Per Year";

        public static List<string> ToListing()
        {
            return new List<string> { PerSecond, PerMinute, PerHour, PerDay, PerMonth, PerQuarter, PerYear };
        }
    }
}
