using EmissionsMonitorModel.TimeSeries;
using System.Text.Json.Serialization;

namespace EmissionsMonitorModel.ProcessModeling
{
    [JsonPolymorphic]
    [JsonDerivedType(typeof(LikeTermsAggregatorNode), typeDiscriminator: "like_term_aggregator")]
    [JsonDerivedType(typeof(ExchangeNode), typeDiscriminator: "exchange_node")]
    public abstract class ProcessNode
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public abstract ICollection<DataFunction> GetUserDefinedFunctions();

        public abstract ProductCostResults RenderProductAndCosts(ICollection<DataPoint> dataPoints);

        public virtual NodeOutputSpec ToSpec()
        {
            return new NodeOutputSpec
            {
                Id = this.Id,
                Name = this.Name,
                Node = this
            };
        }
    }
}
