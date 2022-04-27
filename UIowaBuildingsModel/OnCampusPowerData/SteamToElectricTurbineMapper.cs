using UnitsNet;

namespace UIowaBuildingsModel
{
    public class SteamToElectricTurbineMapper
    {
        public string TurbineName { get; set; }

        public IEnumerable<TaggedDataSet> InvolvedDataSets { get; set; }

        public Func<IEnumerable<TaggedDataPoint>, Energy> TurbineProcessDataToEnergyFunction { get; set; }
    }
}