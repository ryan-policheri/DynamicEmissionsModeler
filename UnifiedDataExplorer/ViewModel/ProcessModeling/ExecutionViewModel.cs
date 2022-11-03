using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
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
        }

        public void Load(int modelId)
        {
            _spec.ModelId = modelId;
        }

        public DateTimeOffset StartTime
        {
            get { return _spec.StartTime; }
            set { _spec.StartTime = value;  OnPropertyChanged();}
        }

        public DateTimeOffset EndTime
        {
            get { return _spec.EndTime; }
            set { _spec.EndTime = value; OnPropertyChanged(); }
        }

        public string Resolution
        {
            get { return _spec.DataResolution;}
            set { _spec.DataResolution = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> AvailableDataResolutions { get; }

        public ICommand ExecuteCommand { get; }

        private async void OnExecute()
        {
            var stuff = await _client.RemoteExecuteAsync(this._spec);
        }
    }
}
