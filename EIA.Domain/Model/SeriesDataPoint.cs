using EIA.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace EIA.Domain.Model
{
    public class SeriesDataPoint
    {//TODO: This needs to be reworked since eia went to v2 of their api
        [JsonPropertyName("period")]
        public string RawTimestamp { get; set; }

        private DateTimeOffset _adhocTimestamp = DateTimeOffset.MinValue;

        [JsonIgnore]
        public DateTimeOffset Timestamp
        {
            get
            {
                if (_adhocTimestamp != DateTimeOffset.MinValue) return _adhocTimestamp;

                //Hourly data
                Regex utcRegex = new Regex("^[1-9]{1}[0-9]{3}-[0-9]{2}-[0-9]{2}T[0-2]{1}[0-9]$");
                Regex offsetRegex = new Regex("^[1-9]{1}[0-9]{7}T[0-2]{1}[0-9][+-][0-9]{2}$");

                //Monthly, Quarterly, Yearly
                Regex monthlyDataRegex = new Regex("^[1-9]{1}[0-9]{3}[0-1]{1}[0-9]{1}$");
                Regex quarterlyDataRegex = new Regex("^[1-9]{1}[0-9]{3}Q[1-4]$");
                Regex yearlyDataRegex = new Regex("^[1-9]{1}[0-9]{3}$");

                List<Regex> matchedRegexes = new List<Regex>();
                DateTimeOffset result;

                if (utcRegex.IsMatch(this.RawTimestamp))
                {
                    result = DateTimeOffset.ParseExact(this.RawTimestamp, "yyyy-MM-ddTHH", CultureInfo.InvariantCulture);
                    if (!matchedRegexes.Contains(utcRegex)) matchedRegexes.Add(utcRegex);
                }
                else if (offsetRegex.IsMatch(this.RawTimestamp))
                {
                    result = DateTime.ParseExact(this.RawTimestamp, "yyyy-MM-ddTHHzz", CultureInfo.InvariantCulture);
                    if (!matchedRegexes.Contains(offsetRegex)) matchedRegexes.Add(offsetRegex);
                }
                else if (monthlyDataRegex.IsMatch(this.RawTimestamp))
                {
                    result = DateTimeOffset.ParseExact(this.RawTimestamp, "yyyyMM", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                    if (!matchedRegexes.Contains(monthlyDataRegex)) matchedRegexes.Add(monthlyDataRegex);
                }
                else if (quarterlyDataRegex.IsMatch(this.RawTimestamp))
                {
                    result = this.RawTimestamp.ParseQuarter();
                    if (!matchedRegexes.Contains(quarterlyDataRegex)) matchedRegexes.Add(quarterlyDataRegex);
                }
                else if (yearlyDataRegex.IsMatch(this.RawTimestamp))
                {
                    result = DateTimeOffset.ParseExact(this.RawTimestamp, "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                    if (!matchedRegexes.Contains(yearlyDataRegex)) matchedRegexes.Add(yearlyDataRegex);
                }
                else throw new NotSupportedException("Do not know how parse the timestamp");

                return result;
            }
            set
            {
                _adhocTimestamp = value;
            }
        }

        [JsonPropertyName("value")]
        public double? Value { get; set; }
    }
}
