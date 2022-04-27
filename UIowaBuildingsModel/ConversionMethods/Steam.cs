using UnitsNet;

namespace UIowaBuildingsModel.ConversionMethods
{
    public static class Steam
    {
        public const double BTUS_PER_POUND_OF_STEAM = 1194; // https://sciencing.com/convert-btu-kw-4911690.html
        public const double BTUS_PER_POUND_OF_STEAM_FROM_WEST_CAMPUS_BOILER = 1168; // Got this value from George

        public static Energy ToEnergyContent(Mass steam, double btusPerPoundOfSteam = BTUS_PER_POUND_OF_STEAM)
        {
            Energy energy = Energy.FromBritishThermalUnits(steam.Pounds * btusPerPoundOfSteam);
            return energy;
        }
    }
}