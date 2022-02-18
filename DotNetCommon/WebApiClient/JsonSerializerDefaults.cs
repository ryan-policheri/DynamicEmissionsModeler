using System.Text.Json;

namespace DotNetCommon.WebApiClient
{
    public static class JsonSerializerDefaults
    {
        public static readonly JsonSerializerOptions CamelCaseOptions = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            IgnoreNullValues = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            IgnoreNullValues = true,
            PropertyNameCaseInsensitive = true
        };
    }
}
