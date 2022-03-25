using System.Data;
using System.Text.Json.Serialization;
using DotNetCommon.Extensions;

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

        [JsonIgnore]
        public DateTimeKind InterpolatedDataTimestampTimeKind => InterpolatedDataPoints?.FirstOrDefault() == null ? DateTimeKind.Unspecified : InterpolatedDataPoints.First().Timestamp.Kind;

        public DataTable RenderDataPointsAsTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add($"TimeStamp ({InterpolatedDataTimestampTimeKind.ToDescription()})", typeof(DateTime));
            table.Columns.Add(EngineeringUnits, typeof(double));

            foreach (var dataPoint in InterpolatedDataPoints)
            {
                DataRow row = table.NewRow();
                row[$"TimeStamp ({InterpolatedDataTimestampTimeKind.ToDescription()})"] = dataPoint.Timestamp;
                row[EngineeringUnits] = dataPoint.Value;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}