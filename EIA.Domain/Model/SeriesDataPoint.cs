using System;

namespace EIA.Domain.Model
{
    public class SeriesDataPoint
    {
        public string RawTimestamp { get; set; }

        public DateTime Timestamp { get; set; }

        public double? Value { get; set; }
    }
}
