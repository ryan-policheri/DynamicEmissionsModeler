using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCommon.Extensions;
using EIA.Domain.Extensions;
using EIA.Domain.Model;
using EmissionsMonitorModel.ProcessModeling;
using PiModel;

namespace EmissionsMonitorModel.TimeSeries
{
    public class TimeSeriesRenderSettings : IBuildPiTimeSeriesQueryString, IBuildEiaTimeSeriesQueryString
    {
        public DateTimeOffset StartDateTime { get; set; }

        public DateTimeOffset EndDateTime { get; set; }

        public string RenderResolution { get; set; }

        //public Interval UnitResolution { get; set; }

        public string BuildPiQueryString()
        {
            string resolution = RenderResolution == null ? "1h" : ResolutionToPiString(RenderResolution);
            string url = ""
                .WithParameter("startTime", StartDateTime.ToStringWithNoOffset())
                .WithParameter("endTime", EndDateTime.ToStringWithNoOffset())
                .WithParameter("interval", resolution);

            return url;
        }

        //private string IntervalToString(Interval interval)
        //{
        //    switch (interval)
        //    {
        //        case Interval.PerSecond: return "1s";
        //        case Interval.PerMinute: return "1m";
        //        case Interval.PerHour: return "1h";
        //        case Interval.PerDay: return "1d";
        //        default: throw new NotImplementedException($"need to implement interval {interval.ToDescription()}");
        //    }
        //}

        private string ResolutionToPiString(string resolution)
        {
            switch (resolution)
            {
                case DataResolution.EverySecond: return "1s";
                case DataResolution.EveryMinute: return "1m";
                case DataResolution.Hourly: return "1h";
                case DataResolution.Daily: return "1d";
                default: throw new NotImplementedException($"need to implement resolution {resolution}");
            }
        }

        public string BuildEiaQueryString()
        {
            string queryString = ""
                .WithQueryString("start", StartDateTime.ToString("yyyyMMddTHHZ"))
                .WithQueryString("end", EndDateTime.ToString("yyyyMMddTHHZ"));
            return queryString;
        }
    }
}
