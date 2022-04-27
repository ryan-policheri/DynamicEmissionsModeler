using UnitsNet;
using PiModel;

namespace UIowaBuildingsModel
{
    public class SteamOverheadFactorMapper
    {
        public string SteamMeterTag { get; set; }

        public Func<InterpolatedDataPoint, Energy> DataPointToSteamEnergyFunction { get; set; }

        public IEnumerable<InterpolatedDataPoint> DataPoints { get; set; }
    }
}
