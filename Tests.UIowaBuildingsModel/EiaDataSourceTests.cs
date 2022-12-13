using System.Collections.Generic;
using System.Linq;
using EmissionsMonitorModel.DataSources;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.EmissionsMonitorModel
{
    [TestClass]
    public class EiaDataSourceTests
    {
        [TestMethod]
        public void WhenModelHasErrors_ValidatorReturnsErrors()
        {
            //var model = new EiaDataSource();
            //model.HasErrors.Should().Be(true);
            //IEnumerable<string> errors = (IEnumerable<string>)model.GetErrors(nameof(EiaDataSource.BaseUrl));
            //errors.Any(x => x.Contains(nameof(EiaDataSource.BaseUrl))).Should().Be(true);
        }
    }
}