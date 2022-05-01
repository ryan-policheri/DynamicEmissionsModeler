namespace PiModel
{
    public class AssetAttribute : ItemBase, IHaveTimeSeriesData
    {
        public string Type { get; set; }
        public string TypeQualifier { get; set; }
        public string DefaultUnitsName { get; set; }
        public string DefaultUnitsNameAbbreviation { get; set; }
        public int DisplayDigits { get; set; }
        public string DataReferencePlugIn { get; set; }
        public string ConfigString { get; set; }
        public bool IsConfigurationItem { get; set; }
        public bool IsExcluded { get; set; }
        public bool IsHidden { get; set; }
        public bool IsManualDataEntry { get; set; }
        public bool HasChildren { get; set; }
        public List<string> CategoryNames { get; set; }
        public bool Step { get; set; }
        public string TraitName { get; set; }
        public double? Span { get; set; }
        public double? Zero { get; set; }

        public AssetAttributeLinks Links { get; set; }

        public IHaveTimeSeriesDataLinks TimeSeriesLinks => Links;

        public IEnumerable<InterpolatedDataPoint> InterpolatedDataPoints { get; set; }

    }
}
