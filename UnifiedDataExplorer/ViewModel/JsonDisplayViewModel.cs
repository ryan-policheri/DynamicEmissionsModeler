using DotNetCommon.DelegateCommand;
using DotNetCommon.EventAggregation;
using DotNetCommon.MVVM;
using System.Windows.Input;
using UnifiedDataExplorer.Events;

namespace UnifiedDataExplorer.ViewModel
{
    public class JsonDisplayViewModel : ViewModelBase
    {
        private readonly IMessageHub _messageHub;

        public JsonDisplayViewModel(IMessageHub messageHub)
        {
            _messageHub = messageHub;
            CloseSeriesCommand = new DelegateCommand(OnCloseSeries);
        }

        private string _header;

        public string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                OnPropertyChanged();
            }
        }

        public string HeaderDetail { get; set; }

        public bool IsCloseable => true;

        public string Json { get; set; }

        public ICommand CloseSeriesCommand { get; }

        private void OnCloseSeries()
        {
            _messageHub.Publish<CloseViewModelEvent>(new CloseViewModelEvent { Sender = this, SenderTypeName = nameof(JsonDisplayViewModel) });
        }
    }
}