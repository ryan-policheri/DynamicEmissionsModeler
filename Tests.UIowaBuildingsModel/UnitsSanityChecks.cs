using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace Tests.UIowaBuildingsModel
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
