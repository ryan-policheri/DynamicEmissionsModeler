namespace PiModel
{
    public interface IBuildPiTimeSeriesQueryString
    {
        public string BuildPiInterpolatedDataQueryString();
        public string BuildPiSummaryDataQueryString();
    }
}
