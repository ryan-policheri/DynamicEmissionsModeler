using System;
using System.Linq;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EIA.Domain.Model;

namespace Tests.EIA.Domain
{
    [TestClass]
    public class SeriesSerializationTests
    {
        [TestMethod]
        public void WhenGivenHourlyUtcTimeSeriesData_ShouldDeserializeCorrectly()
        {
            string json = "{  \"series_id\": \"EBA.MISO-ALL.NG.H\",  \"name\": \"Net generation for Midcontinent Independent System Operator, Inc. (MISO), hourly - UTC time\",  \"units\": \"megawatthours\",  \"f\": \"H\",  \"description\": \"Timestamps follow the ISO8601 standard (https://en.wikipedia.org/wiki/ISO_8601). Hourly representations are provided in Universal Time.\",  \"start\": \"20150701T06Z\",  \"end\": \"20220305T05Z\",  \"updated\": \"2022-03-05T09:26:34-0500\",  \"data\": [    [      \"20220305T05Z\",      65189    ],    [      \"20220305T04Z\",      67427    ],    [      \"20220305T03Z\",      68961    ],    [      \"20220305T02Z\",      69886    ],    [      \"20220305T01Z\",      70906    ],    [      \"20220305T00Z\",      69413    ],    [      \"20220304T23Z\",      68542    ],    [      \"20220304T22Z\",      68419    ],    [      \"20220304T21Z\",      67528    ],    [      \"20220304T20Z\",      66876    ],    [      \"20220304T19Z\",      67415    ],    [      \"20220304T18Z\",      68643    ],    [      \"20220304T17Z\",      70403    ],    [      \"20220304T16Z\",      72889    ],    [      \"20220304T15Z\",      75342    ],    [      \"20220304T14Z\",      76910    ],    [      \"20220304T13Z\",      76269    ],    [      \"20220304T12Z\",      71294    ]  ]}";
            Series series = JsonSerializer.Deserialize<Series>(json);

            Assert.AreEqual("EBA.MISO-ALL.NG.H", series.Id);
            Assert.AreEqual("H", series.Frequency);

            series.ParseAllDates();

            SeriesDataPoint firstDataPoint = series.DataPoints.OrderByDescending(x => x.Timestamp).First();
            SeriesDataPoint lastDataPoint = series.DataPoints.OrderBy(x => x.Timestamp).First();

            Assert.AreEqual(firstDataPoint.Value, 65189);
            Assert.AreEqual(lastDataPoint.Value, 71294);

            Assert.AreEqual(new DateTime(2022, 3, 5, 5, 0, 0, DateTimeKind.Utc), firstDataPoint.Timestamp);
            Assert.AreEqual(new DateTime(2022, 3, 4, 12, 0, 0, DateTimeKind.Utc), lastDataPoint.Timestamp);
        }

        [TestMethod]
        public void WhenGivenHourlyLocalTimeSeriesData_ShouldDeserializeCorrectly()
        {
            string json = "{  \"series_id\": \"EBA.MISO-ALL.NG.HL\",  \"name\": \"Net generation for Midcontinent Independent System Operator, Inc. (MISO), hourly - local time\",  \"units\": \"megawatthours\",  \"f\": \"HL\",  \"description\": \"Timestamps follow the ISO8601 standard (https://en.wikipedia.org/wiki/ISO_8601). Hourly representations are provided in local time for the balancing authority or region.\",  \"start\": \"20150701T01-05\",  \"end\": \"20220304T23-06\",  \"updated\": \"2022-03-05T09:26:34-0500\",  \"data\": [    [      \"20220304T23-06\",      65189    ],    [      \"20220304T22-06\",      67427    ],    [      \"20220304T21-06\",      68961    ],    [      \"20220304T20-06\",      69886    ],    [      \"20220304T19-06\",      70906    ],    [      \"20220304T18-06\",      69413    ],    [      \"20220304T17-06\",      68542    ],    [      \"20220304T16-06\",      68419    ],    [      \"20220304T15-06\",      67528    ],    [      \"20220304T14-06\",      66876    ],    [      \"20220304T13-06\",      67415    ],    [      \"20220304T12-06\",      68643    ],    [      \"20220304T11-06\",      70403    ],    [      \"20220304T10-06\",      72889    ],    [      \"20220304T09-06\",      75342    ],    [      \"20220304T08-06\",      76910    ],    [      \"20220304T07-06\",      76269    ],    [      \"20220304T06-06\",      71294    ],    [      \"20220304T05-06\",      66815    ],    [      \"20220304T04-06\",      64680    ],    [      \"20220304T03-06\",      63594    ],    [      \"20220304T02-06\",      63642    ],    [      \"20220304T01-06\",      64334    ],    [      \"20220304T00-06\",      66328    ],    [      \"20220303T23-06\",      67351    ],    [      \"20220303T22-06\",      69662    ],    [      \"20220303T21-06\",      71612    ],    [      \"20220303T20-06\",      73058    ],    [      \"20220303T19-06\",      72730    ],    [      \"20220303T18-06\",      70515    ],    [      \"20220303T17-06\",      68121    ]  ]}";
            Series series = JsonSerializer.Deserialize<Series>(json);

            Assert.AreEqual("EBA.MISO-ALL.NG.HL", series.Id);
            Assert.AreEqual("HL", series.Frequency);

            series.ParseAllDates();

            SeriesDataPoint firstDataPoint = series.DataPoints.OrderByDescending(x => x.Timestamp).First();
            SeriesDataPoint lastDataPoint = series.DataPoints.OrderBy(x => x.Timestamp).First();

            Assert.AreEqual(firstDataPoint.Value, 65189);
            Assert.AreEqual(lastDataPoint.Value, 68121);

            DateTimeOffset expectedFirstDataPoint = new DateTimeOffset(2022, 3, 4, 23, 0, 0, new TimeSpan(-6, 0, 0));
            DateTimeOffset expectedLastDataPoint = new DateTimeOffset(2022, 3, 3, 17, 0, 0, new TimeSpan(-6, 0, 0));

            Assert.AreEqual(expectedFirstDataPoint.DateTime, firstDataPoint.Timestamp);
            Assert.AreEqual(expectedLastDataPoint.DateTime, lastDataPoint.Timestamp);
        }

        [TestMethod]
        public void WhenGivenQuarterlyTimeSeriesData_ShouldDeserializeCorrectly()
        {
            string json = "{  \"series_id\": \"ELEC.GEN.ALL-AK-99.Q\",  \"name\": \"Net generation : all fuels : Alaska : all sectors : quarterly\",  \"units\": \"thousand megawatthours\",  \"f\": \"Q\",  \"description\": \"Summation of all fuels used for electricity generation; All sectors; \",  \"copyright\": \"None\",  \"source\": \"EIA, U.S. Energy Information Administration\",  \"iso3166\": \"USA-AK\",  \"geography\": \"USA-AK\",  \"start\": \"2001Q1\",  \"end\": \"2021Q4\",  \"updated\": \"2022-02-25T15:25:17-0500\",  \"data\": [    [      \"2021Q4\",      1533.89421    ],    [      \"2021Q3\",      1591.27626    ],    [      \"2021Q2\",      1361.42333    ],    [      \"2021Q1\",      1456.93331    ],    [      \"2020Q4\",      1602.96134    ],    [      \"2020Q3\",      1479.19065    ],    [      \"2020Q2\",      1503.28632    ],    [      \"2020Q1\",      1691.00259    ],    [      \"2019Q4\",      1495.25424    ],    [      \"2019Q3\",      1422.17529    ],    [      \"2019Q2\",      1502.24892    ],    [      \"2019Q1\",      1651.16468    ],    [      \"2018Q4\",      1482.65055    ],    [      \"2018Q3\",      1508.92447    ],    [      \"2018Q2\",      1548.48572    ],    [      \"2018Q1\",      1707.29778    ],    [      \"2017Q4\",      1598.60115    ],    [      \"2017Q3\",      1534.47259    ],    [      \"2017Q2\",      1604.99277    ],    [      \"2017Q1\",      1759.39911    ],    [      \"2016Q4\",      1719.0319    ],    [      \"2016Q3\",      1511.18855    ],    [      \"2016Q2\",      1384.40977    ],    [      \"2016Q1\",      1720.40329    ],    [      \"2015Q4\",      1643.99841    ],    [      \"2015Q3\",      1419.43746    ],    [      \"2015Q2\",      1485.42005    ],    [      \"2015Q1\",      1736.08141    ],    [      \"2014Q4\",      1586.16933    ],    [      \"2014Q3\",      1400.62491    ]  ]}";
            Series series = JsonSerializer.Deserialize<Series>(json);

            Assert.AreEqual("ELEC.GEN.ALL-AK-99.Q", series.Id);
            Assert.AreEqual("Q", series.Frequency);

            series.ParseAllDates();

            SeriesDataPoint firstDataPoint = series.DataPoints.OrderByDescending(x => x.Timestamp).First();
            SeriesDataPoint lastDataPoint = series.DataPoints.OrderBy(x => x.Timestamp).First();

            Assert.AreEqual(firstDataPoint.Value, 1533.89421);
            Assert.AreEqual(lastDataPoint.Value, 1400.62491);

            Assert.AreEqual(new DateTime(2021, 10, 1), firstDataPoint.Timestamp);
            Assert.AreEqual(new DateTime(2014, 7, 1), lastDataPoint.Timestamp);
        }

        [TestMethod]
        public void WhenGivenMonthlyTimeSeriesData_ShouldDeserializeCorrectly()
        {
            string json = "{  \"series_id\": \"ELEC.GEN.ALL-AK-99.M\",  \"name\": \"Net generation : all fuels : Alaska : all sectors : monthly\",  \"units\": \"thousand megawatthours\",  \"f\": \"M\",  \"description\": \"Summation of all fuels used for electricity generation; All sectors; \",  \"copyright\": \"None\",  \"source\": \"EIA, U.S. Energy Information Administration\",  \"iso3166\": \"USA-AK\",  \"geography\": \"USA-AK\",  \"start\": \"200101\",  \"end\": \"202112\",  \"updated\": \"2022-02-25T15:25:17-0500\",  \"data\": [    [      \"202112\",      554.21322    ],    [      \"202111\",      509.61775    ],    [      \"202110\",      470.06324    ],    [      \"202109\",      460.31555    ],    [      \"202108\",      570.71963    ],    [      \"202107\",      560.24108    ],    [      \"202106\",      531.32073    ],    [      \"202105\",      411.97776    ],    [      \"202104\",      418.12484    ],    [      \"202103\",      455.93229    ],    [      \"202102\",      484.03945    ],    [      \"202101\",      516.96157    ],    [      \"202012\",      612.45311    ],    [      \"202011\",      567.25753    ],    [      \"202010\",      423.2507    ],    [      \"202009\",      437.29232    ],    [      \"202008\",      504.94893    ],    [      \"202007\",      536.9494    ],    [      \"202006\",      525.41922    ],    [      \"202005\",      518.05362    ],    [      \"202004\",      459.81348    ],    [      \"202003\",      524.10302    ],    [      \"202002\",      550.45402    ],    [      \"202001\",      616.44555    ],    [      \"201912\",      534.65188    ],    [      \"201911\",      493.59758    ],    [      \"201910\",      467.00478    ],    [      \"201909\",      429.39628    ],    [      \"201908\",      488.8255    ],    [      \"201907\",      503.95351    ],    [      \"201906\",      474.39507    ],    [      \"201905\",      515.18948    ],    [      \"201904\",      512.66437    ],    [      \"201903\",      539.78417    ],    [      \"201902\",      526.67057    ],    [      \"201901\",      584.70994    ],    [      \"201812\",      544.4629    ],    [      \"201811\",      520.26381    ],    [      \"201810\",      417.92384    ],    [      \"201809\",      487.92    ],    [      \"201808\",      506.84993    ],    [      \"201807\",      514.15454    ],    [      \"201806\",      515.23249    ],    [      \"201805\",      539.94781    ],    [      \"201804\",      493.30542    ],    [      \"201803\",      563.56765    ],    [      \"201802\",      544.87858    ],    [      \"201801\",      598.85155    ],    [      \"201712\",      577.73765    ],    [      \"201711\",      545.70776    ],    [      \"201710\",      475.15574    ]  ]}";
            Series series = JsonSerializer.Deserialize<Series>(json);

            Assert.AreEqual("ELEC.GEN.ALL-AK-99.M", series.Id);
            Assert.AreEqual("M", series.Frequency);

            series.ParseAllDates();

            SeriesDataPoint firstDataPoint = series.DataPoints.OrderByDescending(x => x.Timestamp).First();
            SeriesDataPoint lastDataPoint = series.DataPoints.OrderBy(x => x.Timestamp).First();

            Assert.AreEqual(firstDataPoint.Value, 554.21322);
            Assert.AreEqual(lastDataPoint.Value, 475.15574);

            Assert.AreEqual(new DateTime(2021, 12, 1), firstDataPoint.Timestamp);
            Assert.AreEqual(new DateTime(2017, 10, 1), lastDataPoint.Timestamp);
        }

        [TestMethod]
        public void WhenGivenAnnualTimeSeriesData_ShouldDeserializeCorrectly()
        {
            string json = "{  \"series_id\": \"ELEC.GEN.ALL-AK-99.A\",  \"name\": \"Net generation : all fuels : Alaska : all sectors : annual\",  \"units\": \"thousand megawatthours\",  \"f\": \"A\",  \"description\": \"Summation of all fuels used for electricity generation; All sectors; \",  \"copyright\": \"None\",  \"source\": \"EIA, U.S. Energy Information Administration\",  \"iso3166\": \"USA-AK\",  \"geography\": \"USA-AK\",  \"start\": \"2001\",  \"end\": \"2021\",  \"updated\": \"2022-02-25T15:25:17-0500\",  \"data\": [    [      \"2021\",      5943.5271    ],    [      \"2020\",      6276.44089    ],    [      \"2019\",      6070.84313    ],    [      \"2018\",      6247.3585    ],    [      \"2017\",      6497.4656    ],    [      \"2016\",      6335.03351    ],    [      \"2015\",      6284.9373    ],    [      \"2014\",      6042.82963    ],    [      \"2013\",      6496.82164    ],    [      \"2012\",      6946.41882    ],    [      \"2011\",      6871.03279    ],    [      \"2010\",      6759.5757    ],    [      \"2009\",      6702.15939    ],    [      \"2008\",      6774.83438    ],    [      \"2007\",      6821.39162    ],    [      \"2006\",      6674.19677    ],    [      \"2005\",      6576.65854    ],    [      \"2004\",      6526.71686    ],    [      \"2003\",      6338.732    ],    [      \"2002\",      6767.325    ],    [      \"2001\",      6743.766    ]  ]}";
            Series series = JsonSerializer.Deserialize<Series>(json);

            Assert.AreEqual("ELEC.GEN.ALL-AK-99.A", series.Id);
            Assert.AreEqual("A", series.Frequency);

            series.ParseAllDates();

            SeriesDataPoint firstDataPoint = series.DataPoints.OrderByDescending(x => x.Timestamp).First();
            SeriesDataPoint lastDataPoint = series.DataPoints.OrderBy(x => x.Timestamp).First();

            Assert.AreEqual(firstDataPoint.Value, 5943.5271);
            Assert.AreEqual(lastDataPoint.Value, 6743.766);

            Assert.AreEqual(new DateTime(2021, 1, 1), firstDataPoint.Timestamp);
            Assert.AreEqual(new DateTime(2001, 1, 1), lastDataPoint.Timestamp);
        }
    }
}