using UnitsNet;

namespace EmissionsMonitorModel.ConversionMethods
{
    public static class Plastic
    {
        public const double MMBTUS_PER_SHORT_TON = 38; // https://www.ecfr.gov/current/title-40/chapter-I/subchapter-C/part-98/subpart-C/appendix-Table%20C-1%20to%20Subpart%20C%20of%20Part%2098
        public const double KILOGRAMS_PER_MMBTU = 75; // https://www.ecfr.gov/current/title-40/chapter-I/subchapter-C/part-98/subpart-C/appendix-Table%20C-1%20to%20Subpart%20C%20of%20Part%2098

        public static Mass ToCo2Emissions(Mass plastic)
        {
            Energy energyContent = Energy.FromMegabritishThermalUnits(plastic.ShortTons * MMBTUS_PER_SHORT_TON);
            Mass co2 = Mass.FromKilograms(energyContent.MegabritishThermalUnits * KILOGRAMS_PER_MMBTU);
            return co2;
        }
    }
}