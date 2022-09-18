using PiModel;

namespace EmissionsMonitorModel
{
    public class TaggedDataPoint
    {
        public string Tag { get; set; }

        public InterpolatedDataPoint DataPoint { get; set; }
    }
}
