using EmissionsMonitorModel.ProcessModeling;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.EmissionsMonitorModel
{
    [TestClass]
    public class ResolutionAndRatesTests
    {
        [TestMethod]
        public void MoreGranularTests()
        {
            DataResolution.Daily.IsMoreGranularThan(DataResolution.Daily).Should().BeFalse();
            DataResolution.Hourly.IsMoreGranularThan(DataResolution.Daily).Should().BeTrue();
            DataResolution.EveryMinute.IsMoreGranularThan(DataResolution.Daily).Should().BeTrue();
            DataResolution.EverySecond.IsMoreGranularThan(DataResolution.Daily).Should().BeTrue();

            DataResolution.Daily.IsMoreGranularThan(DataResolution.Hourly).Should().BeFalse();
            DataResolution.Hourly.IsMoreGranularThan(DataResolution.Hourly).Should().BeFalse();
            DataResolution.EveryMinute.IsMoreGranularThan(DataResolution.Hourly).Should().BeTrue();
            DataResolution.EverySecond.IsMoreGranularThan(DataResolution.Hourly).Should().BeTrue();

            DataResolution.Daily.IsMoreGranularThan(DataResolution.EveryMinute).Should().BeFalse();
            DataResolution.Hourly.IsMoreGranularThan(DataResolution.EveryMinute).Should().BeFalse();
            DataResolution.EveryMinute.IsMoreGranularThan(DataResolution.EveryMinute).Should().BeFalse();
            DataResolution.EverySecond.IsMoreGranularThan(DataResolution.EveryMinute).Should().BeTrue();

            DataResolution.Daily.IsMoreGranularThan(DataResolution.EverySecond).Should().BeFalse();
            DataResolution.Hourly.IsMoreGranularThan(DataResolution.EverySecond).Should().BeFalse();
            DataResolution.EveryMinute.IsMoreGranularThan(DataResolution.EverySecond).Should().BeFalse();
            DataResolution.EverySecond.IsMoreGranularThan(DataResolution.EverySecond).Should().BeFalse();
        }

        [TestMethod]
        public void LessGranularTests()
        {
            DataResolution.Daily.IsLessGranularThan(DataResolution.Daily).Should().BeFalse();
            DataResolution.Hourly.IsLessGranularThan(DataResolution.Daily).Should().BeFalse();
            DataResolution.EveryMinute.IsLessGranularThan(DataResolution.Daily).Should().BeFalse();
            DataResolution.EverySecond.IsLessGranularThan(DataResolution.Daily).Should().BeFalse();

            DataResolution.Daily.IsLessGranularThan(DataResolution.Hourly).Should().BeTrue();
            DataResolution.Hourly.IsLessGranularThan(DataResolution.Hourly).Should().BeFalse();
            DataResolution.EveryMinute.IsLessGranularThan(DataResolution.Hourly).Should().BeFalse();
            DataResolution.EverySecond.IsLessGranularThan(DataResolution.Hourly).Should().BeFalse();

            DataResolution.Daily.IsLessGranularThan(DataResolution.EveryMinute).Should().BeTrue();
            DataResolution.Hourly.IsLessGranularThan(DataResolution.EveryMinute).Should().BeTrue();
            DataResolution.EveryMinute.IsLessGranularThan(DataResolution.EveryMinute).Should().BeFalse();
            DataResolution.EverySecond.IsLessGranularThan(DataResolution.EveryMinute).Should().BeFalse();

            DataResolution.Daily.IsLessGranularThan(DataResolution.EverySecond).Should().BeTrue();
            DataResolution.Hourly.IsLessGranularThan(DataResolution.EverySecond).Should().BeTrue();
            DataResolution.EveryMinute.IsLessGranularThan(DataResolution.EverySecond).Should().BeTrue();
            DataResolution.EverySecond.IsLessGranularThan(DataResolution.EverySecond).Should().BeFalse();
        }
    }
}
