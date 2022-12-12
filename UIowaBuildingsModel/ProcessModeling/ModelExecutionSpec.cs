using EmissionsMonitorModel.Exceptions;
using System.Text.Json.Serialization;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class ModelExecutionSpec
    {
        public int ModelId { get; set; }
        [JsonIgnore]
        public ProcessModel? Model { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public string DataResolution { get; set; }
        public IEnumerable<int>? NodeIds { get; set; }
        public OverflowHandleStrategies? OverflowHandleStrategy { get; set; }
    }
}
