using UIowaBuildingsModel.ConversionMethods;
using UnitsNet;

namespace UIowaBuildingsModel
{
    public class BuildingUsage
    {
        public DateTimeOffset Timestamp { get; set; }

        public Energy ElectricUsage { get; set; }

        public Mass Co2FromElectricUsage { get; set; }

        public Mass SteamUsageAsMass { get; set; }

        public Energy SteamUsageAsEnergy { get; set; }

        public Mass Co2FromSteamUsage { get; set; }

        public Volume ChilledWaterUsage { get; set; }

        public Mass Co2FromChilledWaterUsage { get; set; }

        public Mass TotalCo2 => Co2FromElectricUsage + Co2FromSteamUsage + Co2FromChilledWaterUsage;

        public Volume Co2InGallonsOfGasolineEquivelent => Gasoline.CalculateGasolineEquivelent(TotalCo2);   
    }
}
