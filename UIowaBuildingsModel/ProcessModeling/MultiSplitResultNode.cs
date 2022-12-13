using EmissionsMonitorModel.TimeSeries;
using System.Text.Json.Serialization;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class MultiSplitResultNode : ProcessNode
    {
        public int OwningSplitterId { get; set; }

        [JsonIgnore]
        public MultiSplitterNode OwningSplitter { get; set; }

        public string SplitFunctionName { get; set; }

        public override ICollection<DataFunction> GetUserDefinedFunctions()
        {
            return new List<DataFunction>();
        }

        public override ProductCostResults RenderProductAndCosts(ICollection<DataPoint> dataPoints)
        {
            return OwningSplitter.RenderSplitFunctionProductAndCosts(dataPoints, this);
        }
    }
}
