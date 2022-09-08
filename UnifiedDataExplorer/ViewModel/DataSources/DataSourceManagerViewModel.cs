using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.EventAggregation;
using DotNetCommon.MVVM;
using EmissionsMonitorModel.DataSources;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.DataSources
{
    public class DataSourceManagerViewModel : RobustViewModelBase
    {
        private ICollection<DataSourceBase> _dataSources;

        public DataSourceManagerViewModel(RobustViewModelDependencies facade) : base(facade)
        {
            _dataSources = new List<DataSourceBase>();
            AddDataSource = new DelegateCommand<DataSourceType?>(OnAddDataSource);
            this.MessageHub.Subscribe<CloseViewModelEvent>(OnCloseViewModelEvent);
        }

        public ObservableCollection<DataSourceBase> DataSources { get; }


        public bool ShowDataSource => SelectedDataSource != null;

        private ViewModelBase _selectedDataSource;
        public ViewModelBase SelectedDataSource
        {
            get { return _selectedDataSource; }
            set { SetField(ref _selectedDataSource, value); OnPropertyChanged(nameof(ShowDataSource)); OnPropertyChanged(nameof(CanAddDataSource)); }
        }

        public bool CanAddDataSource => SelectedDataSource == null;

        public ICommand AddDataSource { get; }

        public ICommand ConfirmAddDataSource { get; }

        public ICommand CancelAddDataSource { get; }

        private async void OnAddDataSource(DataSourceType? sourceType)
        {
            if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));

            switch (sourceType)
            {
                case DataSourceType.Eia:
                    var vm = this.Resolve<EiaDataSourceViewModel>();
                    await vm.LoadAsync();
                    SelectedDataSource = vm;
                    break;
                case DataSourceType.Pi:
                    SelectedDataSource = new PiDataSourceViewModel();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void OnCloseViewModelEvent(CloseViewModelEvent args)
        {
            if(args.SenderTypeName == nameof(EiaDataSourceViewModel) || args.SenderTypeName == nameof(PiDataSourceViewModel)) this.SelectedDataSource = null;
        }
    }
}