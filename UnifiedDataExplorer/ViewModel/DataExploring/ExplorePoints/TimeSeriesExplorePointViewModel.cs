using System;
using System.Data;
using DotNetCommon.EventAggregation;

namespace UnifiedDataExplorer.ViewModel.DataExploring.ExplorePoints
{
    public class TimeSeriesExplorePointViewModel : ExplorePointViewModel
    {
        public TimeSeriesExplorePointViewModel(IMessageHub messageHub) : base(messageHub)
        {
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

        private DateTimeOffset _startDateTime;
        public DateTimeOffset StartDateTimeOffset
        {
            get { return _startDateTime; }
            set { SetField(ref _startDateTime, value); }
        }

        private DateTimeOffset _endDateTimeOffset;
        public DateTimeOffset EndDateTimeOffset
        {
            get { return _endDateTimeOffset; }
            set { SetField(ref _endDateTimeOffset, value); }
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
    }
}
