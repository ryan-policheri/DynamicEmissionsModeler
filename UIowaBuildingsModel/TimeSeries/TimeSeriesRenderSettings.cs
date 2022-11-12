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

        public string BuildPiInterpolatedDataQueryString()
        {
            string resolution = RenderResolution == null ? "1h" : ResolutionToPiString(RenderResolution);
            string url = ""
                .WithParameter("startTime", StartDateTime.ToStringWithNoOffset())
                .WithParameter("endTime", EndDateTime.ToStringWithNoOffset())
                .WithParameter("interval", resolution);

            return url;
        }

        public string BuildPiSummaryDataQueryString()
        {
            string resolution = RenderResolution == null ? "1h" : ResolutionToPiString(RenderResolution);
            DateTimeOffset adjustedForSummaryEnd = AddIntervalUnit(RenderResolution, this.EndDateTime);
            string url = ""
                .WithParameter("startTime", StartDateTime.ToStringWithNoOffset())
                .WithParameter("endTime", adjustedForSummaryEnd.ToStringWithNoOffset())
                .WithParameter("summaryType", "average")
                .WithParameter("summaryDuration", resolution);

            return url;
        }

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

        private DateTimeOffset AddIntervalUnit(string resolution, DateTimeOffset endDateTime)
        {
            switch (resolution)
            {
                case DataResolution.EverySecond: return endDateTime.AddSeconds(1);
                case DataResolution.EveryMinute: return endDateTime.AddMinutes(1);
                case DataResolution.Hourly: return endDateTime.AddHours(1);
                case DataResolution.Daily: return endDateTime.AddDays(1);
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

        public ICollection<DateTimeOffset> BuildTimePoints()
        {
            switch (this.RenderResolution)
            {
                case DataResolution.EverySecond: return this.StartDateTime.EnumerateSecondsUntil(this.EndDateTime);
                case DataResolution.EveryMinute: return this.StartDateTime.EnumerateMinutesUntil(this.EndDateTime);
                case DataResolution.Hourly: return this.StartDateTime.EnumerateHoursUntil(this.EndDateTime);
                default: throw new NotImplementedException("render res " + this.RenderResolution + " not  implemented");
            }
        }
    }
}
