namespace PiModel.Search
{
    public class PiSearchItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PiSearchMatchedField> MatchedFields { get; set; }
        public string ItemType { get; set; }
        public List<object> AFCategories { get; set; }
        public string UniqueID { get; set; }
        public string WebId { get; set; }
        public string UoM { get; set; }
        public string DataType { get; set; }
        public PiSearchItemLinks Links { get; set; }
        public double Score { get; set; }
    }
}