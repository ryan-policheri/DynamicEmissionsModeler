using UnitsNet;

namespace EmissionsMonitorModel.ConversionMethods
{
    public static class Coal
    {
        public const double POUNDS_OF_CO2_PER_POUND_OF_COAL_BURNED = 2.07; //https://www.epa.gov/energy/frequent-questions-epas-greenhouse-gas-equivalencies-calculator

        public const double POUNDS_OF_CO2_PER_KWH_GENERATED_FROM_COAL = 2.23; //https://www.eia.gov/tools/faqs/faq.php?id=74&t=11

        //The following method is a direct translation between burning coal and the emissions caused from that burn
        public static Mass ToCo2Emissions(Mass coal, double poundsOfCo2PerPoundOfCoalBurned = POUNDS_OF_CO2_PER_POUND_OF_COAL_BURNED)
        {
            Mass co2 = Mass.FromPounds(coal.Pounds * poundsOfCo2PerPoundOfCoalBurned);
            return co2;
        }

        //The following method is a translation between how much electricity was generated from coal and how much emissions resulted from that generation.
        //The difference is that to generate electricity the heat energy from the burn must be transformed to electricity (I.E. by using a boiler) which is not 100% efficient.
        //For example if you generated 50 kilowatt hours of electricity from coal, you burned more than 50 kwhs (of heat energy) to do that, so the emission factor is higher.
        //Really the emission factor comes down to how efficient your generator (I.E. boiler) is. The default value is an average for the entire electric grid, which contains generators with varying efficiencies.
        public static Mass ToCo2EmissionsFromElectricGenerated(Energy electricGeneratedFromCoal, double poundsOfCo2PerKwhGeneratedFromCoal = POUNDS_OF_CO2_PER_KWH_GENERATED_FROM_COAL)
        {
            Mass co2 = Mass.FromPounds(electricGeneratedFromCoal.KilowattHours * poundsOfCo2PerKwhGeneratedFromCoal);
            return co2;
        }
    }
}