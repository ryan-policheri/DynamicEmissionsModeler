using UnitsNet;

namespace EmissionsMonitorModel
{
    public class SteamSource
    {
        public DateTimeOffset Timestamp { get; set; }
        public string SourceName { get; set; }
        public IEnumerable<string> InputNames { get; set; }
        public Mass Co2FromInputs { get; set; }
        public Energy SteamEnergyContent { get; set; }
    }
}