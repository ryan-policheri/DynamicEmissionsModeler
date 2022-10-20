using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.EventAggregation;

namespace UnifiedDataExplorer.ViewModel.DataExploring.ExplorePoints
{
    public abstract class TimeSeriesExplorePointViewModel : ExplorePointViewModel
    {
        public TimeSeriesExplorePointViewModel(IMessageHub messageHub) : base(messageHub)
        {
            StartDateTime = DateTime.Today.AddDays(-1);
            EndDateTime = DateTime.Today.AddMinutes(-1);
            RenderDataSetCommand = new DelegateCommand(OnRenderDataSetCommand);
        }

        private string _seriesName;
        public string SeriesName
        {
            get { return _seriesName; }
            set { SetField(ref _seriesName, value); }
        }


        private string _unitsSummary;
        public string UnitsSummary
        {
            get { return _unitsSummary; }
            set { SetField(ref _unitsSummary, value); }
        }

        private DateTime _startDateTime;
        public DateTime StartDateTime
        {
            get { return _startDateTime; }
            set { SetField(ref _startDateTime, value); }
        }

        private DateTime _endDateTime;
        public DateTime EndDateTime
        {
            get { return _endDateTime; }
            set { SetField(ref _endDateTime, value); }
        }

        public ICommand RenderDataSetCommand { get; }

        private DataTable _dataSet;
        public DataTable DataSet
        {
            get { return _dataSet; }
            set { SetField(ref _dataSet, value); }
        }

        private async void OnRenderDataSetCommand()
        {
            await RenderDataSet();
        }

        protected abstract Task RenderDataSet();
    }
}
