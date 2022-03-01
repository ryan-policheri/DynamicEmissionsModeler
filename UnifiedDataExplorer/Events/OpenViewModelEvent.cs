using DotNetCommon.MVVM;

namespace UnifiedDataExplorer.Events
{
    public class OpenViewModelEvent
    {
        public object Sender { get; set; }

        public string SenderTypeName { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public ViewModelBase ViewModel { get; set; }
    }
}
