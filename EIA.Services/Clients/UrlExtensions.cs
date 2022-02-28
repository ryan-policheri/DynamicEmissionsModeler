using System;

namespace EIA.Services.Clients
{
    public static class UrlExtensions
    {
        public static string WithQueryString(this string url, string queryField, string filterValue)
        {
            if (String.IsNullOrWhiteSpace(queryField) || String.IsNullOrWhiteSpace(filterValue)) return url;
            if (!url.Contains("?")) return $"{url}?{queryField}={filterValue}";
            else return $"{url}&{queryField}={filterValue}";
        }
    }
}
