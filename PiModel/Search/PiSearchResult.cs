namespace PiModel.Search
{
    public class PiSearchResult
    {
        public int TotalHits { get; set; }
        public PiSearchLinks Links { get; set; }
        public List<PiSearchError> Errors { get; set; }
        public List<PiSearchItem> Items { get; set; }
    }
}