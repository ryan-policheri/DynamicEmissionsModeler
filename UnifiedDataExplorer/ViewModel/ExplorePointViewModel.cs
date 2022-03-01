using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.EventAggregation;
using DotNetCommon.MVVM;
using UnifiedDataExplorer.Events;

namespace UnifiedDataExplorer.ViewModel
{
    public class ExplorePointViewModel : ViewModelBase
    {
        private readonly IMessageHub _messageHub;

        public ExplorePointViewModel(IMessageHub messageHub)
        {
            _messageHub = messageHub;
            CloseExplorePointCommand = new DelegateCommand(OnCloseExplorePoint);
        }

        private string _header;
        public string Header
        {
            get { return _header; }
            protected set
            {
                _header = value;
                OnPropertyChanged();
            }
        }

        private string _headerDetail;
        public string HeaderDetail
        {
            get { return _headerDetail; }
            protected set
            {
                _headerDetail = value;
                OnPropertyChanged();
            } 
        }

        public bool IsCloseable => true;

        public ICommand CloseExplorePointCommand { get; }

        private void OnCloseExplorePoint()
        {
            _messageHub.Publish<CloseViewModelEvent>(new CloseViewModelEvent { Sender = this, SenderTypeName = nameof(ExplorePointViewModel) });
        }
    }
}