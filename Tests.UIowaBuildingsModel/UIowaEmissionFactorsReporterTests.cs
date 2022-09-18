using System;
using System.Collections.Generic;
using System.Linq;
using DotNetCommon.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiModel;
using EmissionsMonitorModel;
using UnitsNet;

namespace Tests.EmissionsMonitorModel
{
    [TestClass]
    public class UIowaEmissionFactorsReporterTests
    {
        [TestMethod]
        public void WhenConstructingWithStartAndEndTime_ShouldEnumerateCorrectHours()
        {
            DateTimeOffset firstHour = new DateTimeOffset(new DateTime(2022, 3, 31, 8, 0, 0), TimeZones.GetUtcOffset());
            CampusEnergyResourceManager reporter = new CampusEnergyResourceManager(firstHour, firstHour.AddHours(5));

            IEnumerable<DateTimeOffset> hours = reporter.EnumerateHours();

            hours.Count().Should().Be(6);

            hours.ElementAt(0).Should().Be(firstHour);
            hours.ElementAt(1).Should().Be(firstHour.AddHours(1));
            hours.ElementAt(2).Should().Be(firstHour.AddHours(2));
            hours.ElementAt(3).Should().Be(firstHour.AddHours(3));
            hours.ElementAt(4).Should().Be(firstHour.AddHours(4));
            hours.ElementAt(5).Should().Be(firstHour.AddHours(5));
        }

        [TestMethod]
        public void WhenGivenCampusElectricUsageAndCampusElectricGeneration_CorrectlyComputesElectricGenerationAndElectricPurchasePercentagesForAllHours()
        {
            DateTimeOffset firstHour = new DateTimeOffset(new DateTime(2022, 3, 31, 8, 0, 0), TimeZones.GetUtcOffset());
            HourDetails hourDetails = new HourDetails(firstHour);

            List<InterpolatedDataPoint> electricUsageDataPoints = new List<InterpolatedDataPoint>()
            {
                new InterpolatedDataPoint { Timestamp = firstHour, Value = 75 },
                new InterpolatedDataPoint { Timestamp = firstHour.AddHours(1), Value = 100 },
                new InterpolatedDataPoint { Timestamp = firstHour.AddHours(2), Value = 80 },
            };

            List<InterpolatedDataPoint> electricGenerationDataPoints = new List<InterpolatedDataPoint>()
            {
                new InterpolatedDataPoint { Timestamp = firstHour, Value = 25 },
                new InterpolatedDataPoint { Timestamp = firstHour.AddHours(1), Value = 10},
                new InterpolatedDataPoint { Timestamp = firstHour.AddHours(2), Value = 15},
            };

            CampusEnergyResourceManager reporter = new CampusEnergyResourceManager(firstHour, firstHour.AddHours(3));

            DataPointsToEnergyMapper electricUsageMapper = new DataPointsToEnergyMapper
            {
                DataPoints = electricUsageDataPoints,
                DataPointToEnergyFunction = new Func<InterpolatedDataPoint, Energy>((dataPoint) => { return Energy.FromKilowattHours(dataPoint.Value); })
            };

            DataPointsToEnergyMapper electricGenerationMapper = new DataPointsToEnergyMapper
            {
                DataPoints = electricGenerationDataPoints,
                DataPointToEnergyFunction = new Func<InterpolatedDataPoint, Energy>((dataPoint) => { return Energy.FromKilowattHours(dataPoint.Value); })
            };

            reporter.AddCampusElectricUsage(electricUsageMapper);
            reporter.AddCampusElectricGeneration(electricGenerationMapper);

            IEnumerable<HourDetails> hours = reporter.EnumerateHoursDetails();

            //TIMESTAMP ASSERTIONS
            hours.ElementAt(0).Hour.Should().Be(firstHour);
            hours.ElementAt(1).Hour.Should().Be(firstHour.AddHours(1));
            hours.ElementAt(2).Hour.Should().Be(firstHour.AddHours(2));

            //VALUE ASSERTIONS
            hours.ElementAt(0).CampusElectricUsage.KilowattHours.Should().Be(75);
            hours.ElementAt(0).CampusElectricGeneration.KilowattHours.Should().Be(25);
            Math.Round(hours.ElementAt(0).FractionElectricGeneratedOnCampus, 3).Should().Be(0.333);

            hours.ElementAt(1).CampusElectricUsage.KilowattHours.Should().Be(100);
            hours.ElementAt(1).CampusElectricGeneration.KilowattHours.Should().Be(10);
            hours.ElementAt(1).FractionElectricGeneratedOnCampus.Should().Be(0.1);

            hours.ElementAt(2).CampusElectricUsage.KilowattHours.Should().Be(80);
            hours.ElementAt(2).CampusElectricGeneration.KilowattHours.Should().Be(15);
            Math.Round(hours.ElementAt(2).FractionElectricGeneratedOnCampus, 4).Should().Be(0.1875);
        }

        [TestMethod]
        public void WhenGivenElectricGridData_CorrectlyComputesHourlyCarbonEmissions()
        {

        }
    }
}