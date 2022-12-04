using DotNetCommon.Extensions;
using EIA.Domain.Extensions;
using EIA.Domain.Model;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.TimeSeries;
using PiModel;
using System.Text.Json.Serialization;

namespace EmissionsMonitorModel.DataSources
{
    public class DataSourceSeriesUri
    {
        public DataSourceSeriesUri()
        {
            UseZeroForNegatives = true;
            UseZeroForNulls = false;
        }

        public int DataSourceId { get; set; }
        [JsonIgnore]
        public DataSourceBase DataSource { get; set; }

        public string SeriesName { get; set; }

        public string Prefix { get; set;  }

        public string Uri { get; set; }

        public string SeriesUnitsSummary { get; set; }

        public string SeriesUnitRate { get; set; }

        public string SeriesDataResolution { get; set; }

        public bool UseZeroForNegatives { get; set; }

        public bool UseZeroForNulls { get; set; }

        public string FilteringExpression { get; set; }

        public void FillVariableResolution(string renderResolution)
        {
            if (this.SeriesDataResolution != DataResolutionPlusVariable.Variable) throw new InvalidOperationException("Can only replace variable resolution");
            this.SeriesDataResolution = renderResolution;
        }

        public string EquivelentSeriesJoinKey()
        {
            return this.DataSourceId + "_" + this.Prefix + "_" + this.Uri;
        }

        public bool EquivelentSeries(DataSourceSeriesUri other)
        {
            return this.EquivelentSeriesJoinKey() == other.EquivelentSeriesJoinKey();
        }

        public string EquivelentSeriesAndConfigJoinKey(bool includeDataResolution = false)
        {
            var result = this.EquivelentSeriesJoinKey() + "_" + this.SeriesUnitRate + "_";
            if (includeDataResolution) result += this.SeriesDataResolution + "_";
            result += this.UseZeroForNegatives + "_" + this.UseZeroForNulls + "_" + this.FilteringExpression;
            return result;
        }

        public bool EquivelentSeriesAndConfig(DataSourceSeriesUri other)
        {
            if(this.SeriesDataResolution == DataResolutionPlusVariable.Variable || other.SeriesDataResolution == DataResolutionPlusVariable.Variable)
                return this.EquivelentSeriesAndConfigJoinKey(false) == other.EquivelentSeriesAndConfigJoinKey(false);
            else return this.EquivelentSeriesAndConfigJoinKey(true) == other.EquivelentSeriesAndConfigJoinKey(true);
        }
    }

    public class DataSourceSeriesUriQueryRender : DataSourceSeriesUri, IBuildPiTimeSeriesQueryString, IBuildEiaTimeSeriesQueryString
    {
        public DataSourceSeriesUriQueryRender(DataSourceSeriesUri source, DateTimeOffset startTime, DateTimeOffset endTime, string renderResolution)
        {
            this.DataSourceId = source.DataSourceId;
            this.DataSource = source.DataSource?.Copy();
            this.SeriesName = source.SeriesName;
            this.Prefix = source.Prefix;
            this.Uri = source.Uri;
            this.SeriesUnitsSummary = source.SeriesUnitsSummary;
            this.SeriesUnitRate = source.SeriesUnitRate;
            this.SeriesDataResolution = source.SeriesDataResolution;
            this.UseZeroForNegatives = source.UseZeroForNegatives;
            this.UseZeroForNulls = source.UseZeroForNulls;
            this.FilteringExpression = source.FilteringExpression;

            this.StartDateTime = startTime;
            this.EndDateTime = endTime;
            this.RenderResolution = renderResolution;
        }

        public DateTimeOffset StartDateTime { get; set; }

        public DateTimeOffset EndDateTime { get; set; }

        public string RenderResolution { get; set; }

        public string BuildPiInterpolatedDataQueryString()
        {
            string resolution = PiDataSource.ResolutionToPiString(RenderResolution);
            string url = ""
                .WithParameter("startTime", StartDateTime.ToStringWithNoOffset())
                .WithParameter("endTime", EndDateTime.ToStringWithNoOffset())
                .WithParameter("interval", resolution);
            if (!String.IsNullOrWhiteSpace(this.FilteringExpression)) url = url.WithParameter("filterExpression", this.FilteringExpression);

            return url;
        }

        public string BuildPiSummaryDataQueryString()
        {
            string resolution = PiDataSource.ResolutionToPiString(RenderResolution);
            DateTimeOffset adjustedForSummaryEnd = DataResolution.AddIntervalUnit(RenderResolution, this.EndDateTime);
            string url = ""
                .WithParameter("startTime", StartDateTime.ToStringWithNoOffset())
                .WithParameter("endTime", adjustedForSummaryEnd.ToStringWithNoOffset())
                .WithParameter("summaryType", "average")
                .WithParameter("summaryDuration", resolution);
            if (!String.IsNullOrWhiteSpace(this.FilteringExpression)) url = url.WithParameter("filterExpression", this.FilteringExpression);

            return url;
        }

        public string BuildEiaQueryString()
        {
            string queryString = ""
                .WithQueryString("start", StartDateTime.UtcDateTime.ToString("yyyyMMddTHHZ"))
                .WithQueryString("end", EndDateTime.UtcDateTime.ToString("yyyyMMddTHHZ"));
            return queryString;
        }

        public QueryRenderSettings GetQueryRenderSettings()
        {
            return new QueryRenderSettings { StartDateTime = this.StartDateTime, EndDateTime = this.EndDateTime, RenderResolution = this.RenderResolution };
        }
    }
}