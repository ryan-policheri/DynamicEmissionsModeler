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

        public DataTable RenderDataPointsAsTable()
        {
            DataTable table = new DataTable();

            string timeStampString = "TimeStamp (";
            if (InterpolatedDataPoints.FirstOrDefault() != null && InterpolatedDataPoints.First().Timestamp.Offset == TimeZones.GetUtcOffset()) timeStampString += "Utc" + ")";
            else if (InterpolatedDataPoints.FirstOrDefault() != null) timeStampString += "(Local " + InterpolatedDataPoints.First().Timestamp.Offset.ToHourString() + ")";

            string valueString = EngineeringUnits.Replace("/", "");

            table.Columns.Add(timeStampString, typeof(DateTime));
            table.Columns.Add(valueString, typeof(double));

            foreach (var dataPoint in InterpolatedDataPoints)
            {
                DataRow row = table.NewRow();

                if(dataPoint.Timestamp.Offset == TimeZones.GetUtcOffset()) row[timeStampString] = dataPoint.Timestamp.UtcDateTime;
                else row[timeStampString] = dataPoint.Timestamp.LocalDateTime;

                row[valueString] = dataPoint.Value;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}