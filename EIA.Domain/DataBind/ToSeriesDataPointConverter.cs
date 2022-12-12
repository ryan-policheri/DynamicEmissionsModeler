using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using EIA.Domain.Extensions;
using EIA.Domain.Model;

namespace EIA.Domain.DataBind
{
    internal class ToSeriesDataPointConverter : JsonConverter<ICollection<SeriesDataPoint>>
    {
        public override ICollection<SeriesDataPoint> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException("parser was from old version of eia api");
            ICollection<SeriesDataPoint> all = new List<SeriesDataPoint>();
            int startDepth = reader.CurrentDepth;
            int outerArrayDepth = reader.CurrentDepth + 1;
            SeriesDataPoint currentPoint = null;

            //Hourly data
            Regex utcRegex = new Regex("^[1-9]{1}[0-9]{7}T[0-2]{1}[0-9]Z$");
            Regex offsetRegex = new Regex("^[1-9]{1}[0-9]{7}T[0-2]{1}[0-9][+-][0-9]{2}$");

            //Monthly, Quarterly, Yearly
            Regex monthlyDataRegex = new Regex("^[1-9]{1}[0-9]{3}[0-1]{1}[0-9]{1}$");
            Regex quarterlyDataRegex = new Regex("^[1-9]{1}[0-9]{3}Q[1-4]$");
            Regex yearlyDataRegex = new Regex("^[1-9]{1}[0-9]{3}$");

            List<Regex> matchedRegexes = new List<Regex>();

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
                                if (utcRegex.IsMatch(currentPoint.RawTimestamp))
                                {
                                    currentPoint.Timestamp = DateTimeOffset.ParseExact(currentPoint.RawTimestamp, "yyyyMMddTHHZ", CultureInfo.InvariantCulture);
                                    if(!matchedRegexes.Contains(utcRegex)) matchedRegexes.Add(utcRegex);
                                }
                                else if (offsetRegex.IsMatch(currentPoint.RawTimestamp))
                                {
                                    currentPoint.Timestamp = DateTime.ParseExact(currentPoint.RawTimestamp, "yyyyMMddTHHzz", CultureInfo.InvariantCulture);
                                    if (!matchedRegexes.Contains(offsetRegex)) matchedRegexes.Add(offsetRegex);
                                }
                                else if (monthlyDataRegex.IsMatch(currentPoint.RawTimestamp))
                                {
                                    currentPoint.Timestamp = DateTimeOffset.ParseExact(currentPoint.RawTimestamp, "yyyyMM", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                                    if (!matchedRegexes.Contains(monthlyDataRegex)) matchedRegexes.Add(monthlyDataRegex);
                                }
                                else if (quarterlyDataRegex.IsMatch(currentPoint.RawTimestamp))
                                {
                                    currentPoint.Timestamp = currentPoint.RawTimestamp.ParseQuarter();
                                    if (!matchedRegexes.Contains(quarterlyDataRegex)) matchedRegexes.Add(quarterlyDataRegex);
                                }
                                else if (yearlyDataRegex.IsMatch(currentPoint.RawTimestamp))
                                {
                                    currentPoint.Timestamp = DateTimeOffset.ParseExact(currentPoint.RawTimestamp, "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                                    if (!matchedRegexes.Contains(yearlyDataRegex)) matchedRegexes.Add(yearlyDataRegex);
                                }
                                else throw new NotSupportedException("Do not know how parse the timestamp");

                                if (matchedRegexes.Count() > 1) throw new NotSupportedException("The timestamp data has matched more than one timestamp format");
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