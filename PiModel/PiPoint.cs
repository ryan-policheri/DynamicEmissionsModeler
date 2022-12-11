using System.Data;
using System.Text.Json.Serialization;
using DotNetCommon.Extensions;
using DotNetCommon.Helpers;

namespace PiModel
{
    public class PiPoint : IHaveTimeSeriesData
    {
        public const string PI_POINT_TYPE = "PI_POINT";

        public string WebId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Descriptor { get; set; }
        public string PointClass { get; set; }
        public string PointType { get; set; }
        public string DigitalSetName { get; set; }
        public string EngineeringUnits { get; set; }
        public double Span { get; set; }
        public double Zero { get; set; }
        public bool Step { get; set; }
        public bool Future { get; set; }
        public int DisplayDigits { get; set; }
        public PiPointLinks Links { get; set; }

        [JsonIgnore]
        IHaveTimeSeriesDataLinks IHaveTimeSeriesData.TimeSeriesLinks => Links;

        public IEnumerable<InterpolatedDataPoint> InterpolatedDataPoints { get; set; }
        public IEnumerable<SummaryResult> SummaryDataPoints { get; set; }

        public DataTable RenderDataPointsAsTable()
        {
            DataTable table = new DataTable();

            string timeStampString = "TimeStamp (";
            if (SummaryDataPoints.FirstOrDefault() != null && SummaryDataPoints.First().DataPoint.Timestamp.Offset == TimeZones.GetUtcOffset()) timeStampString += "Utc" + ")";
            else if (SummaryDataPoints.FirstOrDefault() != null) timeStampString += "(Local " + SummaryDataPoints.First().DataPoint.Timestamp.Offset.ToHourString() + ")";

            string valueString = String.IsNullOrWhiteSpace(EngineeringUnits) ? "UNKNOWN" : EngineeringUnits.Replace("/", "");

            table.Columns.Add(timeStampString, typeof(DateTime));
            table.Columns.Add(valueString, typeof(double));

            foreach (var dataPoint in SummaryDataPoints)
            {
                DataRow row = table.NewRow();

                if(dataPoint.DataPoint.Timestamp.Offset == TimeZones.GetUtcOffset()) row[timeStampString] = dataPoint.DataPoint.Timestamp.UtcDateTime;
                else row[timeStampString] = dataPoint.DataPoint.Timestamp.LocalDateTime;

                row[valueString] = dataPoint.HasErrors ? DBNull.Value : double.Parse(dataPoint.DataPoint.Value.ToString());
                table.Rows.Add(row);
            }
            return table;
        }
    }
}