namespace PiServices
{
    internal static class UrlExtensions
    {
        public static string WithParameter(this string url, string parameterName, string parameterValue)
        {
            if (String.IsNullOrWhiteSpace(parameterName) || String.IsNullOrWhiteSpace(parameterValue)) return url;
            if (!url.Contains("?")) return $"{url}?{parameterName}={parameterValue}";
            else return $"{url} & {parameterName}={parameterValue}";
        }
    }
}