using System.Text.Json.Serialization;
using DotNetCommon.MVVM;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class NodeOutputSpec : IHaveIntIdAndName
    {
        public NodeOutputSpec()
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public ProcessNode Node { get; set; }
    }
}
