using System.Text.Json.Serialization;
using UnifiedDataExplorer.ModelWrappers;

namespace UnifiedDataExplorer.Events
{
    public class OpenViewModelEvent
    {
        [JsonIgnore]
        public object Sender { get; set; }

        public string SenderTypeName { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Verb { get; set; }

        public string TypeTag { get; set; }
    }

    public class OpenDataSourceViewModelEvent : OpenViewModelEvent, IPiDetailLoadingInfo, IEiaDetailLoadingInfo
    {
        public int DataSourceId { get; set; }
    }
}