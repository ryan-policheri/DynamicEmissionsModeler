using System;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.EventAggregation;
using DotNetCommon.MVVM;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.Services.WindowDialog;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel
{
    public class ModalViewModel : ViewModelBase
    {
        private readonly IMessageHub _messageHub;

        public ModalViewModel(IMessageHub messageHub)
        {
            _messageHub = messageHub;
            _messageHub.Subscribe<CloseViewModelEvent>(OnCloseViewModelEvent);
            SaveProxyCommand = new DelegateCommand(() => SetStatusAndClose(ViewModelDataStatus.Saved));
            DeleteProxyCommand = new DelegateCommand(() => SetStatusAndClose(ViewModelDataStatus.Deleted));
            CancelProxyCommand = new DelegateCommand(() => SetStatusAndClose(ViewModelDataStatus.Canceled));
        }

        public object ChildViewModel { get; set; }
        public Action ClosingCallback { get; set; }
        public ModalOptions Options { get; set; }

        public ICommand SaveProxyCommand { get; }
        public ICommand DeleteProxyCommand { get; }
        public ICommand CancelProxyCommand { get; }

        private void OnCloseViewModelEvent(CloseViewModelEvent args)
        {
            if (this.ChildViewModel != null && args?.Sender != null && this.ChildViewModel == args.Sender)
            {
                SetStatusAndClose(ViewModelDataStatus.Canceled, args);
            }
        }

        private void SetStatusAndClose(ViewModelDataStatus dataStatus, CloseViewModelEvent args = null)
        {
            if (ChildViewModel != null && ChildViewModel is IHaveDataStatus) ((IHaveDataStatus)ChildViewModel).Status = dataStatus;
            if (OnRequestClose == null) return;
            OnRequestClose(this, args);
            if (ClosingCallback != null) ClosingCallback();
        }

        public delegate void RequestClose(object sender, CloseViewModelEvent args);

        public event RequestClose OnRequestClose;
    }
}
