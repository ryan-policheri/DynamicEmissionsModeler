using System;
using System.Data;
using System.Threading.Tasks;
using DotNetCommon.EventAggregation;

namespace UnifiedDataExplorer.ViewModel.DataExploring.ExplorePoints
{
    public abstract class TimeSeriesExplorePointViewModel : ExplorePointViewModel
    {
        public TimeSeriesExplorePointViewModel(IMessageHub messageHub) : base(messageHub)
        {
            StartDateTime = DateTime.Today.AddDays(-1);
            EndDateTime = DateTime.Today.AddMinutes(-1);
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

        private string _unit;
        public string Unit
        {
            get { return _unit; }
            set { SetField(ref _unit, value); }
        }

        private string _unitRate;
        public string UnitRate
        {
            get { return _unit; }
            set { SetField(ref _unit, value); }
        }

        private DataTable _dataSet;
        public DataTable DataSet
        {
            get { return _dataSet; }
            set { SetField(ref _dataSet, value); }
        }

        protected abstract Task RenderDataSet();
    }
}
