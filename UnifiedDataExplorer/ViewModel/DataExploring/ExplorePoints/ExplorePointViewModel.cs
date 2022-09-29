using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.EventAggregation;
using DotNetCommon.MVVM;
using UnifiedDataExplorer.Events;

namespace UnifiedDataExplorer.ViewModel.DataExploring.ExplorePoints
{
    public abstract class ExplorePointViewModel : ViewModelBase
    {
        protected readonly IMessageHub MessageHub;

        public ExplorePointViewModel(IMessageHub messageHub)
        {
            MessageHub = messageHub;
            CloseExplorationItemCommand = new DelegateCommand(OnCloseExplorationItem);
        }

        private string _header;
        public string Header
        {
            get { return _header; }
            protected set { SetField(ref _header, value); }
        }

        private string _headerDetail;
        public string HeaderDetail
        {
            get { return _headerDetail; }
            protected set { SetField(ref _headerDetail, value); }
        }

        public bool IsCloseable => true;

        public object CurrentLoadingInfo { get; protected set; }

        public ICommand CloseExplorationItemCommand { get; }

        private void OnCloseExplorationItem()
        {
            MessageHub.Publish(new CloseViewModelEvent { Sender = this, SenderTypeName = nameof(ExplorePointViewModel) });
        }
    }
}