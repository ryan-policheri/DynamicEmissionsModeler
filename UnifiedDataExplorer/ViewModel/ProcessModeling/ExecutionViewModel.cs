using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.Helpers;
using EmissionsMonitorDataAccess.Http;
using EmissionsMonitorModel.ProcessModeling;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class ExecutionViewModel : RobustViewModelBase
    {
        private readonly ModelExecutionClient _client;
        private ModelExecutionSpec _spec;

        public ExecutionViewModel(ModelExecutionClient client, RobustViewModelDependencies facade) : base(facade)
        {
            _client = client;
            _spec = new ModelExecutionSpec();
            AvailableDataResolutions = new ObservableCollection<string>();
            foreach(string r in DataResolution.ToListing()){ AvailableDataResolutions.Add(r); }
            ExecuteCommand = new DelegateCommand(OnExecute);
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
                if(value != null) SelectedStreamResults = ExecutionResult.NodeSeries.First(x => x.SeriesName == _selectedStream).TransformToDataTable();
            }
        }

        private DataTable _selectStreamResults;
        public DataTable SelectedStreamResults
        {
            get { return _selectStreamResults; }
            set { SetField(ref _selectStreamResults, value); }
        }

        public ICommand ExecuteCommand { get; }

        private async void OnExecute()
        {
            ExecutionResult = await _client.RemoteExecuteAsync(this._spec);
            Streams.Clear();
            foreach(var ns in ExecutionResult.NodeSeries) { Streams.Add(ns.SeriesName); }
        }
    }
}
