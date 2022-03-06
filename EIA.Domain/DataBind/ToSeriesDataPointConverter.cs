using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using EIA.Domain.Model;

namespace EIA.Domain.DataBind
{
    internal class ToSeriesDataPointConverter : JsonConverter<ICollection<SeriesDataPoint>>
    {
        public override ICollection<SeriesDataPoint> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            ICollection<SeriesDataPoint> all = new List<SeriesDataPoint>();
            int startDepth = reader.CurrentDepth;
            int outerArrayDepth = reader.CurrentDepth + 1;
            SeriesDataPoint currentPoint = null;

            while (reader.Read())
            {
                if (reader.CurrentDepth == startDepth) return all;
                else
                {
                    if (reader.TokenType == JsonTokenType.StartArray && reader.CurrentDepth == outerArrayDepth) currentPoint = new SeriesDataPoint();
                    else if (reader.TokenType == JsonTokenType.EndArray && reader.CurrentDepth == outerArrayDepth) all.Add(currentPoint);
                    else
                    {
                        switch (reader.TokenType)
                        {
                            case JsonTokenType.String:
                                currentPoint.RawTimestamp = reader.GetString();
                                break;
                            case JsonTokenType.Number:
                                currentPoint.Value = reader.GetDouble();
                                break;
                            case JsonTokenType.Null:
                                currentPoint.Value = null;
                                break;
                            default:
                                throw new NotImplementedException($"Json type {reader.TokenType.ToString()} conversion to series data element has not been implemented");
                        }
                    }
                }
            }

            throw new JsonException(); // Truncated file or internal error
        }

        public override void Write(Utf8JsonWriter writer, ICollection<SeriesDataPoint> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}