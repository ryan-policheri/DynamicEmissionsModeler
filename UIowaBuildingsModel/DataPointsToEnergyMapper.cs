using PiModel;
using UnitsNet;

namespace UIowaBuildingsModel
{
    public class DataPointsToEnergyMapper
    {
        public IEnumerable<InterpolatedDataPoint> DataPoints { get; set; }
        public Func<InterpolatedDataPoint, Energy> DataPointToEnergyFunction { get; set; }
    }
}