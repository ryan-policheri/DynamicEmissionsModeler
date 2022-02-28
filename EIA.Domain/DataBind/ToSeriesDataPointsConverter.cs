using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using EIA.Domain.Model;

namespace EIA.Domain.DataBind
{
    public class ToSeriesDataPointsConverter : JsonConverter<ICollection<SeriesData>>
    {
        public override ICollection<SeriesData> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            ICollection<SeriesData> all = new List<SeriesData>();
            int startDepth = reader.CurrentDepth;
            SeriesData currentDataArray = null;

            while (reader.Read())
            {
                if (reader.CurrentDepth == startDepth) return all;
                else
                {

                    if (reader.TokenType == JsonTokenType.StartArray) currentDataArray = new SeriesData();
                    else if (reader.TokenType == JsonTokenType.EndArray) all.Add(currentDataArray);
                    else
                    {
                        switch(reader.TokenType)
                        {
                            case JsonTokenType.String:
                                currentDataArray.Add(reader.GetString());
                                break;
                            case JsonTokenType.Number:
                                currentDataArray.Add(reader.GetDouble());
                                break;
                            default:
                                throw new NotImplementedException($"Json type {reader.TokenType.ToString()} conversion to series data element has not been implemented");
                        }
                    }
                }
            }

            throw new JsonException(); // Truncated file or internal error
        }

        public override void Write(Utf8JsonWriter writer, ICollection<SeriesData> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
