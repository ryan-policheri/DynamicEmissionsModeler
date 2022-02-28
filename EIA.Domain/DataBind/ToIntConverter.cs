using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EIA.Domain.DataBind
{
    public class ToIntConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number) return reader.GetInt32();
            else
            {
                string raw = reader.GetString();
                return int.Parse(raw.ToString());
            }
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
