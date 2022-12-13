using UnitsNet;

namespace EmissionsMonitorModel.ConversionMethods
{
    public static class NaturalGas
    {
        //NOTE: A Megagram is the same as a metric ton.
        public const double MEGAGRAMS_OF_CO2_PER_THOUSAND_CUBIC_FOOT_OF_NATURAL_GAS_BURNED = 0.0551; //https://www.epa.gov/energy/greenhouse-gases-equivalencies-calculator-calculations-and-references 
        public const double MEGAGRAMS_OF_CO2_PER_THERM_OF_NATURAL_GAS_BURNED = 0.0053; //https://www.epa.gov/energy/greenhouse-gases-equivalencies-calculator-calculations-and-references

        public const double POUNDS_OF_CO2_PER_KWH_GENERATED_FROM_NATURAL_GAS = 0.91; //https://www.eia.gov/tools/faqs/faq.php?id=74&t=11

        //The following 2 methods are a direct translation between burning natural gas and the emissions caused from that burn
        public static Mass ToCo2Emissions(Volume naturalGas, double megagramsOfCo2PerThousandCubicFootOfNaturalGasBurned = MEGAGRAMS_OF_CO2_PER_THOUSAND_CUBIC_FOOT_OF_NATURAL_GAS_BURNED)
        {
            double kilogramsOfCo2ThousandCubicFeetOfNaturalGas = megagramsOfCo2PerThousandCubicFootOfNaturalGasBurned * 1000; //Convert to kilograms because that's what the library supports.
            Mass co2 = Mass.FromKilograms(naturalGas.KilocubicFeet * kilogramsOfCo2ThousandCubicFeetOfNaturalGas);
            return co2;
        }

        public static Mass ToCo2Emissions(Energy naturalGas, double megagramsOfCo2PerThermOfNaturalGasBurned = MEGAGRAMS_OF_CO2_PER_THERM_OF_NATURAL_GAS_BURNED)
        {
            double kilogramsOfCo2PerThermOfNaturalGas = megagramsOfCo2PerThermOfNaturalGasBurned * 1000; //Convert to kilograms because that's what the library supports.
            Mass co2 = Mass.FromKilograms(naturalGas.ThermsUs * kilogramsOfCo2PerThermOfNaturalGas);
            return co2;
        }

        //The following method is a translation between how much electricity was generated from natural gas and how much emissions resulted from that generation.
        //The difference is that to generate electricity the heat energy from the burn must be transformed to electricity (I.E. by using a boiler) which is not 100% efficient.
        //For example if you generated 50 kilowatt hours of electricity from natural gas, you burned more than 50 kwhs (of heat energy) to do that, so the emission factor is higher.
        //Really the emission factor comes down to how efficient your generator (I.E. boiler) is. The default value is an average for the entire electric grid, which contains generators with varying efficiencies.
        public static Mass ToCo2EmissionsFromElectricGenerated(Energy electricGeneratedFromNaturalGas, double poundsOfCo2PerKwhGeneratedFromNaturalGas = POUNDS_OF_CO2_PER_KWH_GENERATED_FROM_NATURAL_GAS)
        {
            Mass co2 = Mass.FromPounds(electricGeneratedFromNaturalGas.KilowattHours * poundsOfCo2PerKwhGeneratedFromNaturalGas);
            return co2;
        }
    }
}