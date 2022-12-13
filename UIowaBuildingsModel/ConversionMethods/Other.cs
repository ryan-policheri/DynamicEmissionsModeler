using UnitsNet;

namespace EmissionsMonitorModel.ConversionMethods
{
    public static class Other
    {//EIA says the Other contains: "Electricity originating from these sources biomass, fuel cells, geothermal heat, solar power, waste, wind, and wood."
     //Link: https://www.eia.gov/tools/glossary/index.php?id=o
     //So this puts us in a wierd spot because some of these categories (I.E. waste) will have carbon emissions, but others (I.E biomass) will not.
        public const double POUNDS_OF_CO2_PER_KWH_GENERATED_FROM_OTHER = 0.85; //Average grid constant from this source: https://www.eia.gov/tools/faqs/faq.php?id=74&t=11

        public static Mass ToCo2EmissionsFromElectricGenerated(Energy electricGeneratedFromOther, double poundsOfCo2PerKwhGeneratedFromOther = POUNDS_OF_CO2_PER_KWH_GENERATED_FROM_OTHER)
        {
            Mass co2 = Mass.FromPounds(electricGeneratedFromOther.KilowattHours * poundsOfCo2PerKwhGeneratedFromOther);
            return co2;
        }
    }
}