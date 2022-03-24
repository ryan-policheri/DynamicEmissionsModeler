using DotNetCommon.Extensions;
using System.Data;
using System.Text.Json.Serialization;

namespace PiModel
{
    public class AssetValue : ItemBase, IHaveTimeSeriesData
    {
        //This property is only available when loading the list of asset values. When diving into a specific asset value data will not be here
        public Value Value { get; set; }

        //See AssetValueLinks class for more info
        public AssetValueLinks Links { get; set; }

        [JsonIgnore]
        IHaveTimeSeriesDataLinks IHaveTimeSeriesData.TimeSeriesLinks => Links;

        //These properties are only available when diving into a specific asset value. Note the first 2 are commented out because they are a part of ItemBase
        //public string Id { get; set; }
        //public string Description { get; set; }
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

        [JsonIgnore]
        public DateTimeKind InterpolatedDataTimestampTimeKind => InterpolatedDataPoints?.FirstOrDefault() == null ? DateTimeKind.Unspecified : InterpolatedDataPoints.First().Timestamp.Kind;

        public IEnumerable<InterpolatedDataPoint> InterpolatedDataPoints { get; set; }

        public DataTable RenderDataPointsAsTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add($"TimeStamp ({InterpolatedDataTimestampTimeKind.ToDescription()})", typeof(DateTime));
            table.Columns.Add(DefaultUnitsName, typeof(double));

            foreach(var dataPoint in InterpolatedDataPoints)
            {
                DataRow row = table.NewRow();
                row[$"TimeStamp ({InterpolatedDataTimestampTimeKind.ToDescription()})"] = dataPoint.Timestamp;
                row[DefaultUnitsName] = dataPoint.Value;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}