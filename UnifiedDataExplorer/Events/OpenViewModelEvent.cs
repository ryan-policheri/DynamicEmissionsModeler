using UnifiedDataExplorer.ModelWrappers;

namespace UnifiedDataExplorer.Events
{
    public class OpenViewModelEvent : IPiDetailLoadingInfo
    {
        public object Sender { get; set; }

        public string SenderTypeName { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Verb { get; set; }

        public string Tag { get; set; }
    }
}