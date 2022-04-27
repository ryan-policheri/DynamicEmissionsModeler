using UnitsNet;

namespace UIowaBuildingsModel
{
    public class ElectricGridSource
    {
        public DateTimeOffset Timestamp { get; set; }

        public string SourceName { get; set; }

        public string SourceId { get; set; }

        public Mass Co2FromSource { get; set; }

        public Energy ElectricEnergyFromSource { get; set; }
    }
}