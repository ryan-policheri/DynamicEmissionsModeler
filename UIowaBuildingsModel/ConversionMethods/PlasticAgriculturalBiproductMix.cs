using UnitsNet;

namespace EmissionsMonitorModel.ConversionMethods
{
    public static class PlasticAgriculturalBiproductMix
    {
        public const double PERCENT_PLASTIC = 42.5;
        public const double PERCENT_AGRICULTURAL_BIPRODUCT = 57.5;

        public static Mass ToCo2Emissions(Mass plasticAgriculturalMix, double percentPlastic = PERCENT_PLASTIC, double percentAgriculturalBiproduct = PERCENT_AGRICULTURAL_BIPRODUCT)
        {
            if (Math.Round(PERCENT_PLASTIC + PERCENT_AGRICULTURAL_BIPRODUCT) != 100) throw new ArgumentOutOfRangeException("percent plastic and percent agricultural biproduct don't add to 100");
            double decimalPlastic = percentPlastic / 100;
            double decimalAgriculturalBiproduct = percentAgriculturalBiproduct / 100;

            Mass plastic = Mass.FromPounds(plasticAgriculturalMix.Pounds * decimalPlastic);
            Mass agriculturalBiproduct = Mass.FromPounds(plasticAgriculturalMix.Pounds * decimalAgriculturalBiproduct);

            Mass plasticCo2 = Plastic.ToCo2Emissions(plastic);
            Mass agriculturalBiproductCo2 = Mass.Zero; //Considering biomass as zero

            Mass co2 = plasticCo2 + agriculturalBiproductCo2;
            return co2;
        }
    }
}
