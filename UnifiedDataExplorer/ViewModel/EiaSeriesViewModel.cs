using System;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Data;
using DotNetCommon.EventAggregation;
using DotNetCommon.DelegateCommand;
using EIA.Domain.Model;
using EIA.Services.Clients;
using UnifiedDataExplorer.ModelWrappers;
using UnifiedDataExplorer.Services.DataPersistence;
using UIowaBuildingsServices;
using DotNetCommon.SystemHelpers;

namespace UnifiedDataExplorer.ViewModel
{
    public class EiaSeriesViewModel : ExplorePointViewModel
    {
        private readonly EiaClient _client;
        private readonly DataFileProvider _dataFileProvider;
        private readonly ExcelExportService _exporter;

        public EiaSeriesViewModel(EiaClient client,  DataFileProvider dataFileProvider, ExcelExportService exporter, IMessageHub messageHub) : base(messageHub)
        {
            _client = client;
            _dataFileProvider = dataFileProvider;
            _exporter = exporter;
            ExcelExportCommand = new DelegateCommand(OnExcelExport);
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

        private string _valueUnit;
        public string ValueUnit
        { 
            get { return _valueUnit; } 
            private set { SetField<string>(ref _valueUnit, value); }
        }

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

        public ICommand ExcelExportCommand { get; }

        public async Task LoadAsync(IEiaDetailLoadingInfo loadingInfo)
        {
            Series series = await _client.GetSeriesByIdAsync(loadingInfo.Id, 30);
            SeriesName = series.Name;
            SeriesId = series.Id;
            ValueUnit = series.Units;
            Header = series.Id;
            HeaderDetail = this.SeriesName;
            CurrentLoadingInfo = loadingInfo;
            DataSet = series.RenderDataPointsAsTable();
        }

        private void OnExcelExport()
        {
            string exportDirectory = _dataFileProvider.GetExportsDirectory();
            string fileName = "temp" + DateTime.Now.Second.ToString() + ".xlsx";
            string fullPath = SystemFunctions.CombineDirectoryComponents(exportDirectory, fileName);

            _exporter.ExportDataSetToExcel(fullPath, this.DataSet, this.HeaderDetail);
            SystemFunctions.OpenFile(fullPath);
        }
    }
}