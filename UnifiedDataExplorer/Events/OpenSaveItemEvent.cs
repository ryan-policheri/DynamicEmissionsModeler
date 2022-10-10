using EmissionsMonitorModel.VirtualFileSystem;

namespace UnifiedDataExplorer.Events
{
    public class OpenSaveItemEvent
    {
        public object Sender { get; set; }

        public string SenderTypeName { get; set; }

        public SaveItem SaveItem { get; set; }
    }
}
