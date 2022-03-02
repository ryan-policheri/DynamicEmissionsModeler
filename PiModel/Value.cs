using System.Text.Json.Serialization;

namespace PiModel
{
    public class Value
    {
        public DateTime Timestamp { get; set; }
        [JsonPropertyName("Value")]
        public object UntypedValue { get; set; }
        public string UnitsAbbreviation { get; set; }
        public bool Good { get; set; }
        public bool Questionable { get; set; }
        public bool Substituted { get; set; }
        public bool Annotated { get; set; }
    }
}
