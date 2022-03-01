using System.Text.Json;

namespace DotNetCommon.Constants
{
    public static class CommonJsonSerializerOptions
    {
        public static readonly JsonSerializerOptions CaseInsensitive = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public static readonly JsonSerializerOptions CaseInsensitiveBeautiful = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true };
    }
}
