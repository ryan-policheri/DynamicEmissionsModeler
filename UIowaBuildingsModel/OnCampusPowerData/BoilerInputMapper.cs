using PiModel;
using UnitsNet;

namespace EmissionsMonitorModel
{
    public class BoilerInputMapper
    {
        public string InputName { get; set; }

        public string InputTag { get; set; }

        public Func<InterpolatedDataPoint, Mass> DataPointToCo2EmissionsFunction { get; set; }

        public IEnumerable<InterpolatedDataPoint> InputDataPoints { get; set; }
    }
}
