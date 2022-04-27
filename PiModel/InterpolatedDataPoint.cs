using System.Text.Json.Serialization;
using PiModel.DataBind;

namespace PiModel
{
    public class InterpolatedDataPoint 
    {
        public DateTimeOffset Timestamp { get; set; }

        [JsonConverter(typeof(InterpolatedDataPointValueConverter))]
        public double Value { get; set; }
        public string UnitsAbbreviation { get; set; }
        public bool Good { get; set; }
        public bool Questionable { get; set; }
        public bool Substituted { get; set; }
        public bool Annotated { get; set; }
    }
}