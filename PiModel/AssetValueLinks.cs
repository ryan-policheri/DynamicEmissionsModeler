﻿namespace PiModel
{
    public class AssetValueLinks : IHaveTimeSeriesDataLinks
    {
        public string Source { get; set; }

        //The below properties are loaded when going to the source link above

        public string Self { get; set; }
        public string Attributes { get; set; }
        public string Element { get; set; }
        public string Template { get; set; }
        public string InterpolatedData { get; set; }
        public string RecordedData { get; set; }
        public string PlotData { get; set; }
        public string SummaryData { get; set; }
        public string Value { get; set; }
        public string EndValue { get; set; }
        public string Point { get; set; }
        public string Categories { get; set; }
    }
}
