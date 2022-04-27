using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using DotNetCommon.Extensions;
using EIA.Domain.Constants;
using EIA.Domain.DataBind;
using EIA.Domain.Extensions;

namespace EIA.Domain.Model
{
    public class Series
    {
        [JsonPropertyName("series_id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("f")]
        public string Frequency { get; set; }

        [JsonPropertyName("units")]
        public string Units { get; set; }

        [JsonPropertyName("updated")]
        public string Updated { get; set; }

        [JsonPropertyName("data")]
        [JsonConverter(typeof(ToSeriesDataPointConverter))]
        public ICollection<SeriesDataPoint> DataPoints { get; set; }

        public DataTable RenderDataPointsAsTable()
        {
            DataTable table = new DataTable();
            string timestampLabel = "Timestamp (" + GetFrequencyDisplay();
            if (this.Frequency == Frequencies.HOURLY_LOCAL && DataPoints.FirstOrDefault() != null) timestampLabel += " " + DataPoints.First().Timestamp.Offset.ToHourString() + ")";
            else timestampLabel += ")";

            table.Columns.Add(timestampLabel, typeof(DateTime));
            table.Columns.Add(Units, typeof(double));

            foreach (var dataPoint in DataPoints)
            {
                DataRow row = table.NewRow();
                if(this.Frequency == Frequencies.HOURLY_LOCAL) row[timestampLabel] = dataPoint.Timestamp.LocalDateTime;
                else row[timestampLabel] = dataPoint.Timestamp.UtcDateTime;
                row[Units] = dataPoint.Value.HasValue? dataPoint.Value : DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        private string GetFrequencyDisplay()
        {
            switch (this.Frequency)
            {
                case Frequencies.MONTHLY: return "Months";
                case Frequencies.ANNUALLY: return "Years";
                case Frequencies.QUARTERLY: return "Quarters";
                case Frequencies.HOURLY_LOCAL: return "Local Hours";
                case Frequencies.HOURLY_UTC: return "Utc Hours";
                default: throw new NotImplementedException($"Do not know how to get frequency display for {this.Frequency}");
            }
        }
    }
}