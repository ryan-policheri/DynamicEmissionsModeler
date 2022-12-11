using DotNetCommon.Extensions;
using System.Text.Json.Serialization;

namespace PiModel
{
    public class SummaryResult
    {
        [JsonPropertyName("Type")]
        public string SummaryType { get; set; }

        [JsonPropertyName("Value")]
        public SummarizedDataPoint DataPoint { get; set; }

        [JsonIgnore]
        public bool HasErrors => this.DataPoint?.Errors != null && this.DataPoint.Errors.Count() > 0;
    }

    public class SummarizedDataPoint
    {
        public DateTimeOffset Timestamp { get; set; }
        public object Value { get; set; }
        public string UnitsAbbreviation { get; set; }
        public bool Good { get; set; }
        public bool Questionable { get; set; }
        public bool Substituted { get; set; }
        public bool Annotated { get; set; }
        public List<Error> Errors { get; set; }
    }

    public class Error
    {
        public string FieldName { get; set; }
        public List<string> Message { get; set; }

        public override string ToString()
        {
            return FieldName+ ": " + Message.ToDelimitedList(',');
        }
    }
}
