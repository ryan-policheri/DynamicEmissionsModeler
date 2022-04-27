using UnitsNet;

namespace UIowaBuildingsModel
{
    public class SteamOverhead
    {
        public DateTimeOffset Timestamp { get; set; }
        public Energy SteamEnergy { get; set; }
    }
}