using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCommon.Extensions;
using PiModel;

namespace EmissionsMonitorModel.TimeSeries
{
    public class TimeSeriesRenderSettings : IBuildPiTimeSeriesQueryString
    {
        public DateTimeOffset StartDateTime { get; set; }

        public DateTimeOffset EndDateTime { get; set; }

        public Interval UnitResolution { get; set; }

        public string BuildPiQueryString()
        {
            string url = ""
                .WithParameter("startTime", StartDateTime.ToStringWithNoOffset())
                .WithParameter("endTime", EndDateTime.ToStringWithNoOffset())
                .WithParameter("interval", "1h");

            return url;
        }

        private string IntervalToString(Interval interval)
        {
            switch (interval)
            {
                case Interval.PerSecond: return "1s";
                case Interval.PerMinute: return "1m";
                case Interval.PerHour: return "1h";
                case Interval.PerDay: return "1d";
                default: throw new NotImplementedException($"need to implement interval {interval.ToDescription()}");
            }
        }
    }
}
