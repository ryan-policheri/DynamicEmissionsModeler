using System.Text.Json.Serialization;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class NodeOutputSpec
    {
        public NodeOutputSpec()
        {
        }

        public string NodeName { get; set; }

        [JsonIgnore]
        public ProcessNode Node { get; set; }
    }
}
