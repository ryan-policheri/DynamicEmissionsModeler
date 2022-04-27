using DotNetCommon.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PiModel.DataBind
{
    public class InterpolatedDataPointValueConverter : JsonConverter<double>
    {
        public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number) return reader.GetDouble();
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                bool isOutOfService = false;
                while (reader.TokenType != JsonTokenType.EndObject)
                {
                    reader.Read();
                    if(reader.TokenType == JsonTokenType.PropertyName)
                    {
                        if(reader.GetString().CapsAndTrim() == "NAME")
                        {
                            reader.Read();
                            if (reader.TokenType == JsonTokenType.String)
                            {
                                string value = reader.GetString();
                                if (value.CapsAndTrim().OneOf(new string[] { "OUT OF SERV", "CALC FAILED", "SCAN OFF", "SHUTDOWN" }))
                                {
                                    isOutOfService = true;
                                }
                            }    
                        }
                    }
                }
                if (isOutOfService) return 0;
            }
            throw new InvalidDataException("Do not know how to parse json to interpolated data point value");
        }

        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}