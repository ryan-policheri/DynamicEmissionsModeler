using System.Text.Json;

namespace DotNetCommon.Constants
{
    public static class JsonSerializationOptions
    {
        public static readonly JsonSerializerOptions CaseInsensitive = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }
}
