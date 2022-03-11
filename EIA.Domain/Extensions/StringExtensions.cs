using EIA.Domain.Constants;

namespace EIA.Domain.Extensions
{
    public static class StringExtensions
    {
        public static bool IsHourlyFrequency(this string frequencyString)
        {
            return (frequencyString == Frequencies.HOURLY_UTC || frequencyString == Frequencies.HOURLY_LOCAL);
        }
    }
}