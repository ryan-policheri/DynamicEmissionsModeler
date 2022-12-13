using UnitsNet;

namespace EmissionsMonitorModel
{
    public class SteamOverhead
    {
        public DateTimeOffset Timestamp { get; set; }
        public Energy SteamEnergy { get; set; }
    }
}