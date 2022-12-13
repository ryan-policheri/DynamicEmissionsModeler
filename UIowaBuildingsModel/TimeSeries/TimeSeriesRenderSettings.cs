using DotNetCommon.Extensions;
using EIA.Domain.Extensions;
using EIA.Domain.Model;
using EmissionsMonitorModel.DataSources;
using EmissionsMonitorModel.ProcessModeling;
using PiModel;

namespace EmissionsMonitorModel.TimeSeries
{
    public class QueryRenderSettings : IBuildPiTimeSeriesQueryString, IBuildEiaTimeSeriesQueryString
    {
        public DateTimeOffset StartDateTime { get; set; }

        public DateTimeOffset EndDateTime { get; set; }

        public string RenderResolution { get; set; }

        public ICollection<DateTimeOffset> BuildTimePoints() => DataResolution.BuildTimePoints(this.RenderResolution, this.StartDateTime, this.EndDateTime);
        
        public string BuildPiInterpolatedDataQueryString()
        {
            string resolution = RenderResolution == null ? "1h" : PiDataSource.ResolutionToPiString(RenderResolution);
            string url = ""
                .WithParameter("startTime", StartDateTime.ToStringWithNoOffset())
                .WithParameter("endTime", EndDateTime.ToStringWithNoOffset())
                .WithParameter("interval", resolution);

            return url;
        }

        public string BuildPiSummaryDataQueryString()
        {
            string resolution = RenderResolution == null ? "1h" : PiDataSource.ResolutionToPiString(RenderResolution);
            DateTimeOffset adjustedForSummaryEnd = DataResolution.AddIntervalUnit(RenderResolution, this.EndDateTime);
            string url = ""
                .WithParameter("startTime", StartDateTime.ToStringWithNoOffset())
                .WithParameter("endTime", adjustedForSummaryEnd.ToStringWithNoOffset())
                .WithParameter("summaryType", "average")
                .WithParameter("summaryDuration", resolution);

            return url;
        }

        public string BuildEiaQueryString()
        {
            string queryString = ""
                .WithQueryString("start", StartDateTime.UtcDateTime.ToString("yyyyMMddTHHZ"))
                .WithQueryString("end", EndDateTime.UtcDateTime.ToString("yyyyMMddTHHZ"));
            return queryString;
        }
    }
}
