using UnitsNet;
using PiModel;

namespace EmissionsMonitorModel
{
    public class ElectricOverheadFactorMapper
    {
        public bool IsConstantValue { get; set; } = false;
        public string ElectricMeterTag { get; set; }
        public Func<InterpolatedDataPoint, Energy> DataPointToEnergyFunction { get; set; }
        public IEnumerable<InterpolatedDataPoint> DataPoints { get; set; }
    }
}