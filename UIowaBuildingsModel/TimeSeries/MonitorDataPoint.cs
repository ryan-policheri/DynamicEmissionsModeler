using EmissionsMonitorModel.ProcessModeling;

namespace EmissionsMonitorModel.TimeSeries
{
    public class NodeOutputPoint
    {
        public DateTimeOffset Timestamp { get; set; }
        
        public ProductCostResults Values { get; set; }

    }
}
