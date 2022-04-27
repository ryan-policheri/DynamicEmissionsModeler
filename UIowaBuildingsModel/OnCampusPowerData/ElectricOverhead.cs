using UnitsNet;

namespace UIowaBuildingsModel
{
    public class ElectricOverhead
    {
        public DateTimeOffset Timestamp { get; set; }

        public Energy ElectricEnergy { get; set; }
    }
}