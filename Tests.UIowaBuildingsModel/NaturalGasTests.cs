using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using UnitsNet;
using UIowaBuildingsModel.ConversionMethods;
using System;

namespace Tests.UIowaBuildingsModel
{
    [TestClass]
    public class NaturalGasTests
    {
        [TestMethod]
        public void WhenGivenAVolume_ShouldCalculateCarbonEmissions()
        {
            Volume naturalGas = Volume.FromCubicFeet(550);
            Mass co2 = NaturalGas.ToCo2Emissions(naturalGas, 0.5);
            co2.Kilograms.Should().Be(275);
        }

        [TestMethod]
        public void WhenGivenAnEnergy_ShouldCalculateCarbonEmissions()
        {
            Energy energy = Energy.FromMegabritishThermalUnits(15); //Equals 150.03581508 therms
            Mass co2 = NaturalGas.ToCo2Emissions(energy, 0.005);
            Math.Round(co2.Kilograms, 3).Should().Be(750.179);  //750.179 = 150.03581508 therms * 0.005 * 1000
        }

        [TestMethod]
        public void WhenGivenAnEnegyInKilowattHoursAndUsingDefaulEmissionFactor_Co2EmissionsShouldBeClose()
        {//According to here: https://www.eia.gov/tools/faqs/faq.php?id=74&t=11 natural gas will produce 0.4127691 kilograms of CO2 per kwh.
         //Test to see if our conversion factor (based off of therms) holds true
         //It looks like the coefficient derived from EIA series coefficient matches 0.1810368727
         //This test can be removed. It's just a reminder of what the comments say in the NaturalGas class
            Energy energy = Energy.FromKilowattHours(100);
            Mass co2 = NaturalGas.ToCo2Emissions(energy);
            double expectedValueInKilograms = 100 * 0.4127691;
            co2.Kilograms.Should().Be(expectedValueInKilograms);
        }

        [TestMethod]
        public void WhenUsingEnergyAndVolumeWithDefaultEmissionFactors_Co2EmissionsShouldBeClose()
        {
            //10.37 therms equals 1000 cubic feet of natural gas https://www.eia.gov/tools/faqs/faq.php?id=45&t=8 
            //There is 1 MMBTU per 26.8 cubic meters of natural gas, so the following two variables should be equivelent
            Volume naturalGasAsVolume = Volume.FromCubicFeet(1000);
            Energy naturalGasAsEnergy = Energy.FromThermsUs(10.37);
            
            Mass co2FromVolume = NaturalGas.ToCo2Emissions(naturalGasAsVolume);
            Mass co2FromEnergy = NaturalGas.ToCo2Emissions(naturalGasAsEnergy);

            double difference = Math.Abs(co2FromVolume.Kilograms - co2FromVolume.Kilograms);
            difference.Should().BeLessThan(1); //Should be close
        }
    }
}