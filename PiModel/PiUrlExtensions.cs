namespace PiModel
{
    public static class PiUrlExtensions
    {
        public static string WithParameter(this string url, string parameterName, string parameterValue)
        {
            if (String.IsNullOrWhiteSpace(parameterName) || String.IsNullOrWhiteSpace(parameterValue)) return url;
            if (!url.Contains("?")) return $"{url}?{parameterName}={parameterValue}";
            else return $"{url}&{parameterName}={parameterValue}";
        }

        public static string WithQueryParameter(this string url, string parameterName, string parameterValue, string comparator = ":")
        {
            if (String.IsNullOrWhiteSpace(parameterName) || String.IsNullOrWhiteSpace(parameterValue) || String.IsNullOrWhiteSpace(comparator)) return url;
            if (!url.Contains("?")) return $"{url}?q={parameterName}{comparator}{parameterValue}";
            else return $"{url}&{parameterName}{comparator}{parameterValue}";
        }
    }
}
