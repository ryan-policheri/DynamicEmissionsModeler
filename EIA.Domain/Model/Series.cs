using System.Collections.Generic;
using System.Text.Json.Serialization;
using EIA.Domain.DataBind;

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
        [JsonConverter(typeof(ToSeriesDataPointsConverter))]
        public ICollection<SeriesData> Data { get; set; }

        public ConstructedDataSet ToConstructedDataSet()
        {
            return new ConstructedDataSet(this);
        }
    }
}
