using UnitsNet;

namespace UIowaBuildingsModel
{
    public class ChilledWaterProduced
    {
        public DateTimeOffset Timestamp { get; set; }

        public Energy SteamEnergyUsed { get; set; }

        public Energy ElectricEnergyUsed { get; set; }

        public Volume ChilledWaterVolume { get; set; }
    }
}
