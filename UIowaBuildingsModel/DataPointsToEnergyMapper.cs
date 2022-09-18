using PiModel;
using UnitsNet;

namespace EmissionsMonitorModel
{
    public class DataPointsToEnergyMapper
    {
        public IEnumerable<InterpolatedDataPoint> DataPoints { get; set; }
        public Func<InterpolatedDataPoint, Energy> DataPointToEnergyFunction { get; set; }
    }
}