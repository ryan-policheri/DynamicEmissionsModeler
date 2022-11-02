namespace EmissionsMonitorModel.ProcessModeling
{
    public class ExecutionSpec
    {
        public int ModelId { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public string DataResolution { get; set; }
    }
}
