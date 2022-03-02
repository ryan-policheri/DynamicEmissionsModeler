using System.Threading.Tasks;
using System.Data;
using DotNetCommon.EventAggregation;
using EIA.Domain.Model;
using EIA.Services.Clients;
using UnifiedDataExplorer.ModelWrappers;

namespace UnifiedDataExplorer.ViewModel
{
    public class SeriesViewModel : ExplorePointViewModel
    {
        private readonly EiaClient _client;

        public SeriesViewModel(EiaClient client, IMessageHub messageHub) : base(messageHub)
        {
            _client = client;
        }

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

        public string ValueUnit { get; private set; }

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

        public async Task LoadAsync(IEiaDetailLoadingInfo loadingInfo)
        {
            Series series = await _client.GetSeriesByIdAsync(loadingInfo.Id);
            SeriesName = series.Name;
            SeriesId = series.Id;
            ValueUnit = series.Units;
            Header = series.Id;
            HeaderDetail = this.SeriesName;
            CurrentLoadingInfo = loadingInfo;
            ConstructedDataSet dataSet = series.ToConstructedDataSet();
            DataSet = dataSet.Table;
        }
    }
}