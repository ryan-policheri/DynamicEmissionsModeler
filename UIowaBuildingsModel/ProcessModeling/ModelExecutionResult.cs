using EmissionsMonitorModel.TimeSeries;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class ModelExecutionResult
    {
        public ModelExecutionSpec ExecutionSpec { get; set; }
        public ICollection<MonitorSeries> NodeSeries { get; set; }
    }
}
