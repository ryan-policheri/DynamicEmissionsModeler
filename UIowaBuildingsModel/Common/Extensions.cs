using DotNetCommon.Extensions;
using EIA.Domain.Model;
using PiModel;

namespace UIowaBuildingsModel
{
    public static class Extensions
    {
        public static HourDetails FindMatchingHour(this IEnumerable<HourDetails> hoursDetails, DateTimeOffset dateTimeOffset)
        {
            return hoursDetails.First(x => x.Hour.HourMatches(dateTimeOffset));
        }

        public static SeriesDataPoint FindMatchingHour(this IEnumerable<SeriesDataPoint> dataPoints, DateTimeOffset dateTimeOffset)
        {
            return dataPoints.First(x => x.Timestamp.HourMatches(dateTimeOffset));
        }

        public static InterpolatedDataPoint FindMatchingHour(this IEnumerable<InterpolatedDataPoint> dataPoints, DateTimeOffset dateTimeOffset)
        {
            return dataPoints.First(x => x.Timestamp.HourMatches(dateTimeOffset));
        }

        public static InterpolatedDataPoint TryFindMatchingHour(this IEnumerable<InterpolatedDataPoint> dataPoints, DateTimeOffset dateTimeOffset)
        {
            return dataPoints.FirstOrDefault(x => x.Timestamp.HourMatches(dateTimeOffset));
        }
    }
}