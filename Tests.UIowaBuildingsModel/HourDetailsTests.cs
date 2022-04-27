using System;
using System.Collections.Generic;
using DotNetCommon.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitsNet;
using FluentAssertions;
using UIowaBuildingsModel;

namespace Tests.UIowaBuildingsModel
{
    [TestClass]
    public class HourDetailsTests
    {
        [TestMethod]
        public void WhenGivenCampusElectricUsageAndCampusElectricGeneration_CorrectlyComputesElectricGenerationAndElectricPurchasePercentages()
        {
            DateTimeOffset hour = new DateTimeOffset(new DateTime(2022, 3, 31, 8, 0, 0), TimeZones.GetUtcOffset());
            HourDetails hourDetails = new HourDetails(hour);

            Energy electricUsage = Energy.FromMegawattHours(1);
            Energy electricGeneration = Energy.FromKilowattHours(250);

            hourDetails.AddCampusElectricUsage(electricUsage);
            hourDetails.AddCampusElectricGeneration(electricGeneration);

            //Playing around with using different units in the assertions
            hourDetails.CampusElectricUsage.KilowattHours.Should().Be(1000);
            hourDetails.CampusElectricGeneration.MegawattHours.Should().Be(0.25);
            hourDetails.PercentElectricGeneratedOnCampus.Should().Be(25);
            hourDetails.PercentElectricPurchasedFromGrid.Should().Be(75);
            hourDetails.FractionElectricGeneratedOnCampus.Should().Be(0.25);
            hourDetails.FractionElectricPurchasedFromGrid.Should().Be(0.75);
        }

        [TestMethod]
        public void WhenHourDetailsGivenOneSteamSourceAndCogeneratedElectricIsZero_CorrectlyComputesCarbonEmissionsPerUnitOfSteam()
        {
            DateTimeOffset hour = new DateTimeOffset(new DateTime(2022, 3, 31, 8, 0, 0), TimeZones.GetUtcOffset());
            HourDetails hourDetails = new HourDetails(hour);

            //GRID ELECTRIC
            hourDetails.AddElectricGridSource(new ElectricGridSource
            {
                Co2FromSource = Mass.FromKilopounds(1000),
                ElectricEnergyFromSource = Energy.FromMegawattHours(2700),
                SourceId = "UNIT_TEST_GENERIC_GRID_SOURCE",
                SourceName = "Generic Grid Source",
                Timestamp = hour
            });

            //RATIO
            Energy electricUsage = Energy.FromMegawattHours(1);
            Energy electricGeneration = Energy.FromKilowattHours(0);
            hourDetails.AddCampusElectricUsage(electricUsage);
            hourDetails.AddCampusElectricGeneration(electricGeneration);

            //STEAM
            SteamSource steamSource = new SteamSource
            {
                Co2FromInputs = Mass.FromKilograms(150),
                SteamEnergyContent = Energy.FromMegabritishThermalUnits(15)
            };
            hourDetails.AddCampusSteamSource(steamSource);

            //COGENERATED ELECTRIC
            hourDetails.AddCogeneratedElectric(new CogeneratedElectric
            {
                Timestamp = hour,
                SteamEnergyUsed = Energy.FromMegabritishThermalUnits(0),
                SteamTurbineNames = new List<string>() { "UNIT TEST" }
            });

            //STEAM OVERHEAD
            hourDetails.AddSteamOverhead(new SteamOverhead
            {
                Timestamp = hour,
                SteamEnergy = Energy.FromMegabritishThermalUnits(3)
            });

            //ELECTRIC OVERHEAD 
            hourDetails.AddElectricOverhead(new ElectricOverhead { Timestamp = hour, ElectricEnergy = Energy.FromMegawattHours(0.2) });

            Energy steamUsage = Energy.FromMegabritishThermalUnits(0.22);

            //GRID ELECTRIC CO2 FACTOR = 1000 / 2700 = 0.37037 klb/mwh
            //COGENERATED ELECTRIC CO2 FACTOR = 0 - No cogenerated electric in this test
            //CO2 FROM ELECTRIC OVERHEAD = 0.2 * 0.37037 = 0.07407 klb
            //CO2 FROM STEAM = 150 kgs + 0.07407 klbs  = 150 kgs + 33.597586846 klbs  = 183.597586846 kgs of CO2
            //FINAL CO2/STEAM EMISSION FACTOR = 183.597586846 / (15 - 3) = 15.2997989038 kgs/MMBTU

            Mass co2 = hourDetails.CalculateCo2FromSteamUsage(steamUsage); //SHOULD BE 15.2997989038 kgs * 0.22 = 3.36595575884 kgs

            Math.Round(co2.Kilograms, 2).Should().Be(Math.Round(3.36595575884, 2));
        }

        [TestMethod]
        public void WhenHourDetailsGivenAllData_CorrectlyComputesEmissionFactors()
        {
            DateTimeOffset hour = new DateTimeOffset(new DateTime(2022, 4, 5, 5, 0, 0), TimeZones.GetUtcOffset());
            HourDetails hourDetails = new HourDetails(hour);

            //RATIO INFO
            Energy campusElectricUsage = Energy.FromMegawattHours(40);
            Energy campusElectricGeneration = Energy.FromMegawattHours(6);
            hourDetails.AddCampusElectricUsage(campusElectricUsage);
            hourDetails.AddCampusElectricGeneration(campusElectricGeneration);

            //GRID INFO
            hourDetails.AddElectricGridSource(new ElectricGridSource 
            {
                Co2FromSource = Mass.FromKilopounds(20000), ElectricEnergyFromSource = Energy.FromMegawattHours(25000), SourceId = "UNIT_TEST_NATURAL_GAS", SourceName = "Natural Gas", Timestamp = hour
            });
            hourDetails.AddElectricGridSource(new ElectricGridSource 
            {
                Co2FromSource = Mass.FromKilopounds(40000), ElectricEnergyFromSource = Energy.FromMegawattHours(20000), SourceId = "UNIT_TEST_COAL", SourceName = "Coal", Timestamp = hour
            });
            hourDetails.AddElectricGridSource(new ElectricGridSource 
            {
                Co2FromSource = Mass.FromKilopounds(0), ElectricEnergyFromSource = Energy.FromMegawattHours(2000), SourceId = "UNIT_TEST_HYDRO", SourceName = "Hydro", Timestamp = hour
            });
            hourDetails.AddElectricGridSource(new ElectricGridSource 
            {
                Co2FromSource = Mass.FromKilopounds(0), ElectricEnergyFromSource = Energy.FromMegawattHours(15000), SourceId = "UNIT_TEST_WIND", SourceName = "Wind", Timestamp = hour
            });

            //CAMPUS PRODUCTION
            hourDetails.AddCampusSteamSource(new SteamSource 
            {
                Co2FromInputs = Mass.FromKilograms(8000), SteamEnergyContent = Energy.FromMegabritishThermalUnits(75), Timestamp = hour,
                InputNames = new List<string>() { "UNIT TEST" }, SourceName = "UNIT_TEST_BOILER_10"
            });
            hourDetails.AddCampusSteamSource(new SteamSource 
            {
                Co2FromInputs = Mass.FromKilograms(4000), SteamEnergyContent = Energy.FromMegabritishThermalUnits(70), Timestamp = hour,
                InputNames = new List<string>() { "UNIT TEST" }, SourceName = "UNIT_TEST_BOILER_7"
            });
            hourDetails.AddCampusSteamSource(new SteamSource 
            {
                Co2FromInputs = Mass.FromKilograms(0), SteamEnergyContent = Energy.FromMegabritishThermalUnits(0), Timestamp = hour,
                InputNames = new List<string>() { "UNIT TEST" }, SourceName = "UNIT_TEST_OFF_BOILER"
            });

            //COGENERATED ELECTRIC
            hourDetails.AddCogeneratedElectric(new CogeneratedElectric 
            { 
                Timestamp = hour,
                SteamEnergyUsed = Energy.FromMegabritishThermalUnits(20),
                SteamTurbineNames = new List<string>() { "UNIT TEST" } 
            });

            //STEAM OVERHEAD
            hourDetails.AddSteamOverhead(new SteamOverhead
            {
                Timestamp = hour,
                SteamEnergy = Energy.FromMegabritishThermalUnits(30)
            });

            //ELECTRIC OVERHEAD 
            hourDetails.AddElectricOverhead(new ElectricOverhead { Timestamp = hour, ElectricEnergy = Energy.FromMegawattHours(1) });

            //CHILLED WATER
            hourDetails.AddChilledWaterGeneration(new ChilledWaterProduced
            {
                Timestamp = hour,
                ChilledWaterVolume = Volume.FromCubicMeters(80 * 60),
                ElectricEnergyUsed = Energy.FromMegawattHours(6),
                SteamEnergyUsed = Energy.FromMegabritishThermalUnits(40)
            });

            Energy buildingElectricUsage = Energy.FromKilowattHours(50);
            Energy buildingSteamUsage = Energy.FromMegabritishThermalUnits(1);
            Volume buildingChilledWaterUsage = Volume.FromUsGallons(50 * 60);

            Mass co2FromSteamUsage = hourDetails.CalculateCo2FromSteamUsage(buildingSteamUsage);
            Mass co2FromElectricUsage = hourDetails.CalculateCo2EmissionsFromElectricUsage(buildingElectricUsage);
            Mass co2FromChilledWaterUsage = hourDetails.CalculateCo2EmissionsFromChilledWaterUsage(buildingChilledWaterUsage);

            Mass expectedCo2FromElectricUsage = Mass.FromKilograms(21.356);
            Mass expectedco2FromSteamUsage = Mass.FromKilograms(108.062);
            Mass expectedCo2FromChilledWaterUsage = Mass.FromKilograms(16.289);

            Math.Round(co2FromElectricUsage.Kilograms, 2).Should().Be(Math.Round(expectedCo2FromElectricUsage.Kilograms, 2));
            Math.Round(co2FromSteamUsage.Kilograms, 2).Should().Be(Math.Round(expectedco2FromSteamUsage.Kilograms, 2));
            Math.Round(co2FromChilledWaterUsage.Kilograms, 2).Should().Be(Math.Round(expectedCo2FromChilledWaterUsage.Kilograms, 2));
        }
    }
}