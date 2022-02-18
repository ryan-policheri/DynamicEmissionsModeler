using System.Text.Json;

namespace DotNetCommon.WebApiClient
{
    public static class HttpResponseMessageExtensions
    {

        public static async Task<T> AsAsync<T>(this HttpResponseMessage response, JsonSerializerOptions serializerOptions, string nestedPropertyToStartFrom)
        {
            if (String.IsNullOrWhiteSpace(nestedPropertyToStartFrom)) return await response.AsAsync<T>(serializerOptions);

            Stream originalStream = await response.EnsureSuccessStatusCode().Content.ReadAsStreamAsync();
            JsonDocument jdoc = await JsonDocument.ParseAsync(originalStream);
            JsonElement jsonElement = jdoc.RootElement.GetProperty(nestedPropertyToStartFrom);
            string raw = jsonElement.GetRawText();
            return JsonSerializer.Deserialize<T>(raw, serializerOptions);
        }

        public static async Task<T> AsAsync<T>(this HttpResponseMessage response, JsonSerializerOptions serializerOptions)
        {
#if DEBUG
            string payload = await AsStringAsync(response);
            return JsonSerializer.Deserialize<T>(payload, serializerOptions);
#else
            Task<Stream> streamTask = response.EnsureSuccessStatusCode().Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(await streamTask, serializerOptions);
#endif
        }

        public static Task<T> AsAsync<T>(this HttpResponseMessage response)
        {
            return AsAsync<T>(response, JsonSerializerDefaults.CamelCaseOptions);
        }

        public static async Task<byte[]> AsBytesAsync(this HttpResponseMessage response)
        {
            return await response.EnsureSuccessStatusCode().Content.ReadAsByteArrayAsync();
        }

        public static async Task<string> AsStringAsync(this HttpResponseMessage response)
        {
            return await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
        }
    }
}
