using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.Helpers;
using DotNetCommon.SystemHelpers;
using EmissionsMonitorDataAccess.Http;
using EmissionsMonitorModel.ProcessModeling;
using UIowaBuildingsServices;
using UnifiedDataExplorer.Services.DataPersistence;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class ExecutionViewModel : RobustViewModelBase
    {
        private readonly ModelExecutionClient _client;
        private readonly DataFileProvider _dataFileProvider;
        private readonly ExcelExportService _exporter;
        private ModelExecutionSpec _spec;

        public ExecutionViewModel(ModelExecutionClient client, DataFileProvider dataFileProvider, ExcelExportService exporter, RobustViewModelDependencies facade) : base(facade)
        {
            _client = client;
            _dataFileProvider = dataFileProvider;
            _exporter = exporter;
            _spec = new ModelExecutionSpec();

            AvailableDataResolutions = new ObservableCollection<string>();
            foreach(string r in DataResolution.ToListing()){ AvailableDataResolutions.Add(r); }
            ExecuteCommand = new DelegateCommand(OnExecute);
            ExcelExportCommand = new DelegateCommand(OnExcelExport);
            StartTime = DateTime.Today.AddDays(-1);
            EndTime = DateTime.Today.AddSeconds(-1);

            Streams = new ObservableCollection<string>();
        }

        public void Load(int modelId)
        {
            _spec.ModelId = modelId;
        }

        public DateTime StartTime
        {
            get { return _spec.StartTime.LocalDateTime; }
            set { _spec.StartTime = new DateTimeOffset(value, TimeZones.GetCentralTimeOffset(value));  OnPropertyChanged();}
        }

        public DateTime EndTime
        {
            get { return _spec.EndTime.LocalDateTime; }
            set { _spec.EndTime = new DateTimeOffset(value, TimeZones.GetCentralTimeOffset(value)); ; OnPropertyChanged(); }
        }

        public string Resolution
        {
            get { return _spec.DataResolution;}
            set { _spec.DataResolution = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> AvailableDataResolutions { get; }

        private ModelExecutionResult _executionResult;
        public ModelExecutionResult ExecutionResult
        {
            get { return _executionResult; }
            set 
            { 
                SetField(ref _executionResult, value);
                OnPropertyChanged(nameof(ResultsAvailable));
            }
        }

        public bool ResultsAvailable => ExecutionResult != null;

        public ObservableCollection<string> Streams { get; }


        private string _selectedStream;
        public string SelectedStream
        {
            get { return _selectedStream; }
            set 
            {
                SetField(ref _selectedStream, value); 
                if(value != null) SelectedStreamResults = ExecutionResult.NodeSeries.First(x => x.NodeName == _selectedStream).TransformToDataTable();
            }
        }

        private DataTable _selectStreamResults;
        public DataTable SelectedStreamResults
        {
            get { return _selectStreamResults; }
            set { SetField(ref _selectStreamResults, value); }
        }

        public ICommand ExecuteCommand { get; }

        public ICommand ExcelExportCommand { get; }

        private async void OnExecute()
        {
            ExecutionResult = await _client.RemoteExecuteAsync(this._spec);
            Streams.Clear();
            foreach(var ns in ExecutionResult.NodeSeries) { Streams.Add(ns.NodeName); }
        }

        private void OnExcelExport()
        {
            string exportDirectory = _dataFileProvider.GetExportsDirectory();
            string fileName = "temp" + DateTime.Now.Second.ToString() + ".xlsx";
            string fullPath = SystemFunctions.CombineDirectoryComponents(exportDirectory, fileName);

            _exporter.ExportExecutionResult(fullPath, _executionResult);
            SystemFunctions.OpenFile(fullPath);
        }
    }
}
