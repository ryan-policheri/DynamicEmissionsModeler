using EmissionsMonitorModel.ConversionMethods;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace Tests.EmissionsMonitorModel
{
    [TestClass]
    public class PlasticTests
    {
        [TestMethod]
        public void WhenGivenMassOfPlastic_CorrectlyComputesEmissionsFromBurn()
        {
            var plasticMass = Mass.FromPounds(1);
            var emissiosn = Plastic.ToCo2Emissions(plasticMass);
            emissiosn.Kilograms.Should().Be(1.425);
        }
    }
}
