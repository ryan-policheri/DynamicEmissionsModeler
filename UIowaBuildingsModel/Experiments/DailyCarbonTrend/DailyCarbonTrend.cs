namespace EmissionsMonitorModel.Experiments.DailyCarbonTrend
{
    public class DailyCarbonTrendConfig
    {
        public int ModelId { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }
    }
}
