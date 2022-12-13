using EmissionsMonitorModel.TimeSeries;

namespace EmissionsMonitorModel.Exceptions
{
    public class NodeOverflowError
    {
        public DateTimeOffset TimeStamp { get; set; }

        public int NodeId { get; set; }

        public string NodeName { get; set; }

        public IEnumerable<DataPoint> NodeInputs { get; set; }

    }
}
