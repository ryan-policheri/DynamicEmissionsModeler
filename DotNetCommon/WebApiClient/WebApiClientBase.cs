using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text.Json;

namespace DotNetCommon.WebApiClient
{
    public abstract class WebApiClientBase
    {
        public HttpClient Client { get; protected set; }

        protected JsonSerializerOptions SerializerOptions { get; set; }

        protected string NestedPropertyToStartFrom { get; set; }

        protected WebApiClientBase()
        {

        }

        protected WebApiClientBase(HttpClient httpClient)
        {
            Client = httpClient;
        }

        public async Task<string> GetAsync(string uri)
        {
            var result = await InternalGetAsync(uri);
            return result;
        }

        public async Task<T> GetAsync<T>(string uri)
        {
            var result = await InternalGetAsync<T>(uri, null);
            return result;
        }

        public async Task<T> GetAsync<T>(string uri, string nestedPropertyToStartFrom)
        {
            var result = await InternalGetAsync<T>(uri, nestedPropertyToStartFrom);
            return result;
        }

        public async Task<T> GetFirstAsync<T>(string uri)
        {
            var result = await InternalGetFirstAsync<T>(uri, null);
            return result;
        }

        public async Task<T> GetFirstAsync<T>(string uri, string nestedPropertyToStartFrom)
        {
            var result = await InternalGetFirstAsync<T>(uri, nestedPropertyToStartFrom);
            return result;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(string uri)
        {
            var result = await InternalGetAllAsync<T>(uri, null);
            return result;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(string uri, string nestedPropertyToStartFrom)
        {
            var result = await InternalGetAllAsync<T>(uri, nestedPropertyToStartFrom);
            return result;
        }

        public async Task<T> PostAsync<T>(string uri, T data, IDictionary<string, string> headers = null)
        {
            var result = await InternalPostAsync(uri, data, headers);
            return result;
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string uri, TRequest data, IDictionary<string, string> headers, Func<HttpResponseMessage, Task<TResponse>> func)
        {
            try
            {
                byte[] bodyContent = JsonSerializer.SerializeToUtf8Bytes(data, SerializerOptions);

                ByteArrayContent byteContent = new ByteArrayContent(bodyContent);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                MaybeApplyHeaders(byteContent, headers);

                using HttpResponseMessage response = await Client.PostAsync(PrepareUri(uri), byteContent);

                return await func.Invoke(response);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string uri, TRequest data, Func<HttpResponseMessage, Task<TResponse>> func)
        {
            return await PostAsync(uri, data, null, func);
        }

        public async Task<T> PostDataAndFile<T>(string uri, T obj, string filePath)
        {
            FileInfo info = new FileInfo(filePath);

            using (MultipartFormDataContent content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
            {
                content.Add(new StreamContent(File.OpenRead(filePath)), "file", info.Name);
                StringContent stringContent = new StringContent(JsonSerializer.Serialize<T>(obj));
                content.Add(stringContent, "jsonString");

                using (HttpResponseMessage response = await Client.PostAsync(uri, content))
                {
                    return await response.AsAsync<T>(SerializerOptions);
                }
            }
        }

        public async Task<T> PutAsync<T>(string uri, T data, IDictionary<string, string> headers = null)
        {
            var result = await InternalPutAsync(uri, data, headers);
            return result;
        }

        public async Task<TResponse> PutAsync<TRequest, TResponse>(string uri, TRequest data, IDictionary<string, string> headers, Func<HttpResponseMessage, Task<TResponse>> func)
        {
            try
            {
                byte[] bodyContent = JsonSerializer.SerializeToUtf8Bytes(data, SerializerOptions);

                ByteArrayContent byteContent = new ByteArrayContent(bodyContent);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                MaybeApplyHeaders(byteContent, headers);

                using HttpResponseMessage response = await Client.PutAsync(PrepareUri(uri), byteContent);

                return await func.Invoke(response);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TResponse> PutAsync<TRequest, TResponse>(string uri, TRequest data, Func<HttpResponseMessage, Task<TResponse>> func)
        {
            return await PutAsync(uri, data, null, func);
        }

        public async Task<T> DeleteAsync<T>(string uri)
        {
            if (Client == null) throw new InvalidOperationException("No http client is set for this api repository");
            var result = await InternalDeleteAsync(uri);

            return await JsonSerializer.DeserializeAsync<T>(new MemoryStream(result), SerializerOptions);
        }

        public async Task DeleteAsync(string uri)
        {
            await InternalDeleteAsync(uri);
        }

        public async Task<byte[]> GetRawAsync(String uri)
        {
            using HttpResponseMessage response = await Client.GetAsync(PrepareUri(uri));
            return await response.AsBytesAsync();
        }

        public async Task<byte[]> GetRawResourceAsync(string fullUrl)
        {
            using HttpResponseMessage response = await Client.GetAsync(fullUrl);
            return await response.AsBytesAsync();
        }

        private async Task<string> InternalGetAsync(string uri)
        {
            try
            {
                using HttpResponseMessage response = await Client.GetAsync(PrepareUri(uri));
                return await response.AsStringAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<T> InternalGetAsync<T>(string uri, string nestedPropertyToStartFrom)
        {
            if (String.IsNullOrWhiteSpace(nestedPropertyToStartFrom)) nestedPropertyToStartFrom = NestedPropertyToStartFrom;
            try
            {
                using HttpResponseMessage response = await Client.GetAsync(PrepareUri(uri));
                string responseString = await response.AsStringAsync();
                return await response.AsAsync<T>(SerializerOptions, nestedPropertyToStartFrom);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<T> InternalGetFirstAsync<T>(string uri, string nestedPropertyToStartFrom)
        {
            if (String.IsNullOrWhiteSpace(nestedPropertyToStartFrom)) nestedPropertyToStartFrom = NestedPropertyToStartFrom;
            try
            {
                string jsonResult = string.Empty;
                using HttpResponseMessage response = await Client.GetAsync(PrepareUri(uri));
                var result = await response.AsAsync<IEnumerable<T>>(SerializerOptions, nestedPropertyToStartFrom);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<IEnumerable<T>> InternalGetAllAsync<T>(string uri, string nestedPropertyToStartFrom)
        {
            if (String.IsNullOrWhiteSpace(nestedPropertyToStartFrom)) nestedPropertyToStartFrom = NestedPropertyToStartFrom;

            using HttpResponseMessage response = await Client.GetAsync(PrepareUri(uri));
#if DEBUG
            var sample = await response.AsStringAsync();
#endif
            var result = await response.AsAsync<IEnumerable<T>>(SerializerOptions, nestedPropertyToStartFrom);
            return result;
        }

        private async Task<T> InternalPostAsync<T>(string uri, T data, IDictionary<string, string> headers = null)
        {
            try
            {
                byte[] bodyContent = JsonSerializer.SerializeToUtf8Bytes(data, SerializerOptions);

                ByteArrayContent byteContent = new ByteArrayContent(bodyContent);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                MaybeApplyHeaders(byteContent, headers);

                using HttpResponseMessage response = await Client.PostAsync(PrepareUri(uri), byteContent);
                string responseString = await response.AsStringAsync();

                return await response.AsAsync<T>(SerializerOptions, NestedPropertyToStartFrom);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<T> InternalPutAsync<T>(string uri, T data, IDictionary<string, string> headers = null)
        {
            try
            {
                String body = JsonSerializer.Serialize(data, SerializerOptions);

                byte[] bodyContent = JsonSerializer.SerializeToUtf8Bytes(data, SerializerOptions);

                ByteArrayContent byteContent = new ByteArrayContent(bodyContent);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                MaybeApplyHeaders(byteContent, headers);

                using HttpResponseMessage response = await Client.PutAsync(PrepareUri(uri), byteContent);

                return await response.AsAsync<T>(SerializerOptions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<byte[]> InternalDeleteAsync(string uri)
        {
            try
            {
                using HttpResponseMessage response = await Client.DeleteAsync(PrepareUri(uri));
                return await response.AsBytesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static async Task HandleErrorResponse(HttpResponseMessage response)
        {
            string message = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.Unauthorized ||
                response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new AuthenticationException(message);
            }

            throw new HttpRequestException(message);
        }

        public string PrepareUri(string uri)
        {
            string newUri = uri;
            string baseAddress = Client.BaseAddress.ToString();
            if (!newUri.StartsWith(baseAddress))
            {
                if (!newUri.StartsWith('/'))
                {
                    newUri = "/" + newUri;
                }
                if (!string.IsNullOrEmpty(baseAddress))
                {
                    if (baseAddress.EndsWith('/'))
                    {
                        baseAddress = baseAddress.Substring(0, baseAddress.Length - 1);
                    }
                    newUri = baseAddress + newUri;
                }
            }
            return newUri;
        }

        private void MaybeApplyHeaders(HttpContent content, IDictionary<string, string> headers)
        {
            if (headers != null && content != null)
            {
                foreach (var pair in headers)
                {
                    content.Headers.Add(pair.Key, pair.Value);
                }
            }
        }
    }
}