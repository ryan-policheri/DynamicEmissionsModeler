using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.EventAggregation;
using DotNetCommon.SystemHelpers;
using EmissionsMonitorModel.DataSources;
using UIowaBuildingsServices;
using UnifiedDataExplorer.Services.DataPersistence;

namespace UnifiedDataExplorer.ViewModel.DataExploring.ExplorePoints
{
    public abstract class TimeSeriesExplorePointViewModel : ExplorePointViewModel
    {
        private readonly DataFileProvider _dataFileProvider;
        private readonly ExcelExportService _exporter;

        public TimeSeriesExplorePointViewModel(DataFileProvider dataFileProvider, ExcelExportService exporter, IMessageHub messageHub) : base(messageHub)
        {
            _dataFileProvider = dataFileProvider;
            _exporter = exporter;
            StartDateTime = DateTime.Today.AddDays(-1);
            EndDateTime = DateTime.Today.AddMinutes(-1);
            RenderDataSetCommand = new DelegateCommand(OnRenderDataSetCommand);
            ExcelExportCommand = new DelegateCommand(OnExcelExport);
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


        private string _seriesDescription;
        public string SeriesDescription
        {
            get { return _seriesDescription; }
            set { SetField(ref _seriesDescription, value); }
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

        private DataTable _dataSet;
        public DataTable DataSet
        {
            get { return _dataSet; }
            set { SetField(ref _dataSet, value); }
        }

        public ICommand RenderDataSetCommand { get; }

        private async void OnRenderDataSetCommand()
        {
            await RenderDataSet();
        }

        protected abstract Task RenderDataSet();

        public ICommand ExcelExportCommand { get; }

        private void OnExcelExport()
        {
            string exportDirectory = _dataFileProvider.GetExportsDirectory();
            string fileName = "temp" + DateTime.Now.Second.ToString() + ".xlsx";
            string fullPath = SystemFunctions.CombineDirectoryComponents(exportDirectory, fileName);

            _exporter.ExportDataSetToExcel(fullPath, DataSet, HeaderDetail);
            SystemFunctions.OpenFile(fullPath);
        }

        public abstract DataSourceSeriesUri BuildSeriesUri();
    }
}
