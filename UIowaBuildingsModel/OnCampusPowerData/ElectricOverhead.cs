using UnitsNet;

namespace EmissionsMonitorModel
{
    public class ElectricOverhead
    {
        public DateTimeOffset Timestamp { get; set; }

        public Energy ElectricEnergy { get; set; }
    }
}