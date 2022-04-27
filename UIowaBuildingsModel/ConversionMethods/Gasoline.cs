using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace UIowaBuildingsModel.ConversionMethods
{
    public static class Gasoline
    {
        const double GRAMS_OF_CO2_PER_GALLON_OF_GASOLINE = 8887; //Source: https://www.epa.gov/energy/greenhouse-gases-equivalencies-calculator-calculations-and-references#gasoline

        public static Volume CalculateGasolineEquivelent(Mass co2Emissions)
        {
            return Volume.FromUsGallons(co2Emissions.Grams / GRAMS_OF_CO2_PER_GALLON_OF_GASOLINE);
        }
    }
}
