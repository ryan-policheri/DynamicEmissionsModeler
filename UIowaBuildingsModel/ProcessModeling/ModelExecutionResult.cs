using EmissionsMonitorModel.Exceptions;
using EmissionsMonitorModel.TimeSeries;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class ModelExecutionResult
    {
        public ModelExecutionSpec ExecutionSpec { get; set; }
        public ICollection<NodeSeries> NodeSeries { get; set; }
        public ICollection<NodeOverflowError>? Errors { get; set; }
    }
}
