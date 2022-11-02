using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using EmissionsMonitorModel.ProcessModeling;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class ExecutionViewModel : RobustViewModelBase
    {
        private ExecutionSpec _spec;

        public ExecutionViewModel(RobustViewModelDependencies facade) : base(facade)
        {
            _spec = new ExecutionSpec();
            AvailableDataResolutions = new ObservableCollection<string>();
            foreach(string r in DataResolution.ToListing()){ AvailableDataResolutions.Add(r); }
            ExecuteCommand = new DelegateCommand(OnExecute);
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

        private void OnExecute()
        {
            //throw new NotImplementedException();
        }
    }
}
