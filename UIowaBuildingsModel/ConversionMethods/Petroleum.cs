using UnitsNet;

namespace EmissionsMonitorModel.ConversionMethods
{
    public static class Petroleum
    {
        public const double POUNDS_OF_CO2_PER_KWH_GENERATED_FROM_PETRO = 2.13; //https://www.eia.gov/tools/faqs/faq.php?id=74&t=11

        public static Mass ToCo2EmissionsFromElectricGenerated(Energy electricGeneratedFromPetro, double poundsOfCo2PerKwhGeneratedFromPetro = POUNDS_OF_CO2_PER_KWH_GENERATED_FROM_PETRO)
        {
            Mass co2 = Mass.FromPounds(electricGeneratedFromPetro.KilowattHours * poundsOfCo2PerKwhGeneratedFromPetro);
            return co2;
        }
    }
}