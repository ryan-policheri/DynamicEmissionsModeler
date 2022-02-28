using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.EventAggregation;
using DotNetCommon.MVVM;
using EIA.Domain.Model;
using EIA.Services.Clients;
using EIADataViewer.Events;
using System.Data;

namespace EIADataViewer.ViewModel
{
    public class SeriesViewModel : ViewModelBase
    {
        private readonly EiaClient _client;
        private readonly IMessageHub _messageHub;

        public SeriesViewModel(EiaClient client, IMessageHub messageHub)
        {
            _client = client;
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

        public string HeaderDetail => SeriesName;

        public bool IsCloseable => true;

        private string _seriesName;
        public string SeriesName
        {
            get { return _seriesName; }
            private set
            {
                _seriesName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HeaderDetail));
            }
        }

        public string SeriesId { get; private set; }

        private DataTable _dataSet;
        public DataTable DataSet 
        {
            get { return _dataSet; }
            private set
            {
                _dataSet = value;
                OnPropertyChanged();
            }
        }

        public ICommand CloseSeriesCommand { get; }

        public async Task LoadAsync(string seriesId)
        {
            Series series = await _client.GetSeriesByIdAsync(seriesId);
            SeriesName = series.Name;
            SeriesId = series.Id;
            Header = series.Id;
            ConstructedDataSet dataSet = series.ToConstructedDataSet();
            DataSet = dataSet.Table;
        }

        private void OnCloseSeries()
        {
            _messageHub.Publish<CloseViewModelEvent>(new CloseViewModelEvent { Sender = this, SenderTypeName = nameof(SeriesViewModel) });
        }
    }
}
