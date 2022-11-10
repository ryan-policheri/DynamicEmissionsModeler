using EmissionsMonitorModel.ProcessModeling;
using System.Text.Json.Serialization;

namespace EmissionsMonitorModel.DataSources
{
    public class DataSourceSeriesUri
    {
        public int DataSourceId { get; set; }
        [JsonIgnore]
        public DataSourceBase DataSource { get; set; }

        public string SeriesName { get; set; }

        public string Prefix { get; set;  }

        public string Uri { get; set; }

        public string SeriesUnitsSummary { get; set; }

        public string SeriesUnitRate { get; set; }

        public string SeriesDataResolution { get; set; }

        public void FillVariableResolution(string renderResolution)
        {
            if (this.SeriesDataResolution != DataResolutionPlusVariable.Variable) throw new InvalidOperationException("Can only replace variable resolution");
            this.SeriesDataResolution = renderResolution;
        }
    }
}