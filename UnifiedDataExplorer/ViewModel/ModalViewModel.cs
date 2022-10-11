using DotNetCommon.EventAggregation;
using DotNetCommon.MVVM;
using UnifiedDataExplorer.Events;

namespace UnifiedDataExplorer.ViewModel
{
    public class ModalViewModel : ViewModelBase
    {
        private readonly IMessageHub _messageHub;

        public ModalViewModel(IMessageHub messageHub)
        {
            _messageHub = messageHub;
            _messageHub.Subscribe<CloseViewModelEvent>(OnCloseViewModelEvent);
        }

        public object ChildViewModel { get; set; }

        private void OnCloseViewModelEvent(CloseViewModelEvent args)
        {
            if (this.ChildViewModel != null && args?.Sender != null && this.ChildViewModel == args.Sender)
            {
                if (OnRequestClose == null) return;
                OnRequestClose(this, args);
            }
        }


        public delegate void RequestClose(object sender, CloseViewModelEvent args);

        public event RequestClose OnRequestClose;
    }
}
