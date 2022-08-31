using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using EmissionsMonitorModel.DataSources;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel
{
    public class DataSourceManagerViewModel : RobustViewModelBase
    {
        private ICollection<DataSourceBase> _dataSources;

        public DataSourceManagerViewModel(RobustViewModelDependencies facade) : base(facade)
        {
            _dataSources = new List<DataSourceBase>();
            AddDataSource = new DelegateCommand<DataSourceType?>(OnAddDataSource);
        }

        public ObservableCollection<DataSourceBase> DataSources { get; }


        public bool ShowDataSource => SelectedDataSource != null;

        private DataSourceBase _selectedDataSource;
        public DataSourceBase SelectedDataSource
        {
            get { return _selectedDataSource; }
            set { SetField(ref _selectedDataSource, value); OnPropertyChanged(nameof(ShowDataSource)); }
        }

        public ICommand AddDataSource { get; }

        private void OnAddDataSource(DataSourceType? sourceType)
        {
            if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));

            switch (sourceType)
            {
                case DataSourceType.Eia:
                    SelectedDataSource = new EiaDataSource();
                    break;
                case DataSourceType.Pi:
                    SelectedDataSource = new PiDataSource();
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}