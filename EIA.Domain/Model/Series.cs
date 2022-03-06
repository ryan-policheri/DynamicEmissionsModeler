using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text.Json.Serialization;
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

        public void ParseAllDates()
        {//Could also do this in the JsonConverter using regexes and such but it's convenient to know the Frequency
            CultureInfo us = new CultureInfo("en-US");
            foreach (SeriesDataPoint dataPoint in DataPoints)
            {
                switch (this.Frequency)
                {
                    case "M":
                        dataPoint.Timestamp = DateTime.ParseExact(dataPoint.RawTimestamp, "yyyyMM", us);
                        break;
                    case "A":
                        dataPoint.Timestamp = DateTime.ParseExact(dataPoint.RawTimestamp, "yyyy", us);
                        break;
                    case "Q":
                        dataPoint.Timestamp = dataPoint.RawTimestamp.ParseQuarter();
                        break;
                    case "HL":
                        dataPoint.Timestamp = DateTime.ParseExact(dataPoint.RawTimestamp, "yyyyMMddTHHzz", us);
                        break;
                    case "H":
                        dataPoint.Timestamp = DateTime.ParseExact(dataPoint.RawTimestamp, "yyyyMMddTHHZ", us, DateTimeStyles.AdjustToUniversal);
                        break;
                    default:
                        throw new NotImplementedException($"Do not know how to parse date time for the given frequency: {this.Frequency}");
                }
            }
        }

        public DataTable RenderDataPointsAsTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Timestamp", typeof(DateTime));
            table.Columns.Add(Units, typeof(double));

            foreach (var dataPoint in DataPoints)
            {
                DataRow row = table.NewRow();
                row["Timestamp"] = dataPoint.Timestamp;
                row[Units] = dataPoint.Value.HasValue? dataPoint.Value : DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
