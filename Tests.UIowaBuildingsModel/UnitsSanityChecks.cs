using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitsNet;

namespace Tests.EmissionsMonitorModel
{
    [TestClass]
    public class UnitsSanityChecks
    {
        [TestMethod]
        public void WhenUsingUnitsNetMegaBtus_ShouldBeTheSameAsMetricMillionBtus()
        {//Test to confirm that what UnitsNet calls MegabritishThermalUnits is the same as MMBTU
         //15 MMBTUs is equal to 15,000,000 btus
            Energy mmbtu = Energy.FromMegabritishThermalUnits(15);
            mmbtu.BritishThermalUnits.Should().Be(15000000);
        }
    }
}
