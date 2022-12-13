using EmissionsMonitorModel.TimeSeries;
using System.Text.Json.Serialization;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class StreamSplitResultNode : ProcessNode
    {
        public int OwningSplitterId { get; set; }

        [JsonIgnore]
        public StreamSplitterNode OwningSplitter { get; set; }

        public override ICollection<DataFunction> GetUserDefinedFunctions()
        {
            return new List<DataFunction>();
        }

        public override ProductCostResults RenderProductAndCosts(ICollection<DataPoint> dataPoints)
        {
            return OwningSplitter.RenderSplitFunctionProductAndCosts(dataPoints);
        }
    }
}
