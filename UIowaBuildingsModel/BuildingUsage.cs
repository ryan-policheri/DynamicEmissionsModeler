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

        public Volume TotalCo2InGasolineVolumeEquivelent => Gasoline.CalculateGasolineEquivelent(TotalCo2);
        public Volume ElectricCo2InGasolineVolumeEquivelent => Gasoline.CalculateGasolineEquivelent(Co2FromElectricUsage);
        public Volume HeatingCo2InGasolineVolumeEquivelent => Gasoline.CalculateGasolineEquivelent(Co2FromSteamUsage);
        public Volume CoolingCo2InGasolineVolumeEquivelent => Gasoline.CalculateGasolineEquivelent(Co2FromChilledWaterUsage);
    }
}
