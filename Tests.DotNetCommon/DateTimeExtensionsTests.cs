using System;
using DotNetCommon.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.DotNetCommon
{
    [TestClass]
    public class DateTimeExtensionsTests
    {
        [TestMethod]
        public void WhenGivenALocalTime_ShouldConvertToLocalTimeStringWithOffset()
        {//TODO: this test will fail outside of central timezone.

            DateTime localStandardTimeDate = new DateTime(2022, 3, 10, 2, 14, 33, DateTimeKind.Local);
            DateTime localDaylightSavingsTimeDate = new DateTime(2022, 3, 15, 18, 22, 49, DateTimeKind.Local);
            
            string localStandardTimeResult = localStandardTimeDate.ToStringWithLocalOffset();
            string localDaylightTimeResult = localDaylightSavingsTimeDate.ToStringWithLocalOffset();

            string localStandardTimeExpected = "2022-03-10T02:14:33-06:00";
            string localDaylightTimeExpected = "2022-03-15T18:22:49-05:00";
            
            Assert.AreEqual(localStandardTimeExpected, localStandardTimeResult);
            Assert.AreEqual(localDaylightTimeExpected, localDaylightTimeResult);
        }

        [TestMethod]
        public void WhenGivenAUtcTime_ShouldConvertToLocalTimeStringWithOffset()
        {
            DateTime utcTime1 = new DateTime(2022, 3, 10, 2, 14, 33, DateTimeKind.Utc);
            DateTime utcTime2 = new DateTime(2022, 3, 15, 18, 22, 49, DateTimeKind.Utc);

            string localStandardTimeResult = utcTime1.ToStringWithLocalOffset();
            string localDaylightTimeResult = utcTime2.ToStringWithLocalOffset();

            string localStandardTimeExpected = "2022-03-09T20:14:33-06:00";
            string localDaylightTimeExpected = "2022-03-15T13:22:49-05:00";

            Assert.AreEqual(localStandardTimeExpected, localStandardTimeResult);
            Assert.AreEqual(localDaylightTimeExpected, localDaylightTimeResult);
        }

        [TestMethod]
        public void WhenGivenAUtcTime_ShouldConvertToStringWithNoOffset()
        {
            DateTime utcTime = new DateTime(2022, 3, 10, 2, 14, 33, DateTimeKind.Utc);

            string expectedString = "2022-03-10T02:14:33Z";
            string actualString = utcTime.ToStringWithNoOffset();

            Assert.AreEqual(expectedString, actualString);
        }

        [TestMethod]
        public void WhenGivenALocalTime_ShouldConvertToStringWithNoOffset()
        {//TODO: this test will fail outside of central timezone.
            DateTime localStandardTimeDate = new DateTime(2022, 3, 10, 2, 14, 33, DateTimeKind.Local);
            DateTime localDaylightSavingsTimeDate = new DateTime(2022, 3, 15, 18, 22, 49, DateTimeKind.Local);

            string localStandardTimeExpected = "2022-03-10T08:14:33Z";
            string localDaylightTimeExpected = "2022-03-15T23:22:49Z";

            string localStandardTimeActual = localStandardTimeDate.ToStringWithNoOffset();
            string localDaylightTimeActual = localDaylightSavingsTimeDate.ToStringWithNoOffset();

            Assert.AreEqual(localStandardTimeExpected, localStandardTimeActual);
            Assert.AreEqual(localDaylightTimeExpected, localDaylightTimeActual);
        }
    }
}
