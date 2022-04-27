using UnitsNet;

namespace UIowaBuildingsModel.ConversionMethods
{
    public static class Plastic
    {
        public const double POUNDS_OF_CO2_PER_POUND_OF_PLASTIC_BURNED = 1; //TODO: Use these numbers: https://www.ecfr.gov/current/title-40/chapter-I/subchapter-C/part-98/subpart-C/appendix-Table%20C-1%20to%20Subpart%20C%20of%20Part%2098

        public static Mass ToCo2Emissions(Mass plastic, double poundsOfCo2PerPoundOfPlasticBurned = POUNDS_OF_CO2_PER_POUND_OF_PLASTIC_BURNED)
        {
            Mass co2 = Mass.FromPounds(plastic.Pounds * poundsOfCo2PerPoundOfPlasticBurned);
            return co2;
        }
    }
}