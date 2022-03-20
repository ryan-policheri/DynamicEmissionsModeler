using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text.Json.Serialization;
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

        public void ParseAllDates()
        {//Could also do this in the JsonConverter using regexes and such but it's convenient to know the Frequency
            foreach (SeriesDataPoint dataPoint in DataPoints)
            {
                switch (this.Frequency)
                {
                    case Frequencies.MONTHLY:
                        dataPoint.Timestamp = DateTime.ParseExact(dataPoint.RawTimestamp, "yyyyMM", CultureInfo.InvariantCulture);
                        break;
                    case Frequencies.ANNUALLY:
                        dataPoint.Timestamp = DateTime.ParseExact(dataPoint.RawTimestamp, "yyyy", CultureInfo.InvariantCulture);
                        break;
                    case Frequencies.QUARTERLY:
                        dataPoint.Timestamp = dataPoint.RawTimestamp.ParseQuarter();
                        break;
                    case Frequencies.HOURLY_LOCAL:
                        dataPoint.Timestamp = DateTime.ParseExact(dataPoint.RawTimestamp, "yyyyMMddTHHzz", CultureInfo.InvariantCulture);
                        break;
                    case Frequencies.HOURLY_UTC:
                        dataPoint.Timestamp = DateTime.ParseExact(dataPoint.RawTimestamp, "yyyyMMddTHHZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
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
