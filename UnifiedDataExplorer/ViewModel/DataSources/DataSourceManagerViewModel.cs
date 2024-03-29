﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.MVVM;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.DataSources;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.DataSources
{
    public class DataSourceManagerViewModel : RobustViewModelBase
    {
        private readonly IDataSourceRepository _repo;

        public DataSourceManagerViewModel(IDataSourceRepository repo, RobustViewModelDependencies facade) : base(facade)
        {
            _repo = repo;
            DataSources = new ObservableCollection<DataSourceBase>();
            AddDataSource = new DelegateCommand<DataSourceType?>(OnAddDataSource);
            this.MessageHub.Subscribe<CloseViewModelEvent>(OnCloseViewModelEvent);
            this.MessageHub.Subscribe<SaveViewModelEvent>(OnSaveViewModelEvent);
        }

        public ObservableCollection<DataSourceBase> DataSources { get; }


        private DataSourceBase _selectedDataSourceBase;
        public DataSourceBase SelectedDataSourceBase
        {
            get { return _selectedDataSourceBase; }
            set
            {
                SetField(ref _selectedDataSourceBase, value);
                OnDataSourceSelected(value);
            }
        }

        public bool ShowDataSource => SelectedDataSource != null;

        private ViewModelBase _selectedDataSource;
        public ViewModelBase SelectedDataSource
        {
            get { return _selectedDataSource; }
            set
            {
                SetField(ref _selectedDataSource, value); 
                OnPropertyChanged(nameof(ShowDataSource)); 
                OnPropertyChanged(nameof(CanAddDataSource));
            }
        }

        public bool CanAddDataSource => SelectedDataSource == null;

        public ICommand AddDataSource { get; }

        public async Task LoadAsync()
        {
            IEnumerable<DataSourceBase> dataSources = await _repo.GetAllDataSourcesAsync();
            DataSources.Clear();
            foreach(DataSourceBase dataSource in dataSources) DataSources.Add(dataSource);
        }

        private void OnAddDataSource(DataSourceType? sourceType)
        {
            if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));
            SelectDataSource(sourceType.Value);
        }

        private void OnDataSourceSelected(DataSourceBase args)
        {
            if (args == null) return;
            SelectDataSource(args.SourceType, args);
        }

        private void SelectDataSource(DataSourceType sourceType, DataSourceBase model = null)
        {
            DataSourceBaseViewModel untypedVm = this.Resolve<DataSourceBaseViewModel>();
            var vm = untypedVm.InitializeSubclassViewModel(sourceType, model);
            SelectedDataSource = vm;
        }

        private async void OnSaveViewModelEvent(SaveViewModelEvent args)
        {
            if (args.SenderTypeName == nameof(EiaDataSourceViewModel) || args.SenderTypeName == nameof(PiDataSourceViewModel))
            {
                this.SelectedDataSource = null;
                await this.LoadAsync();
            }
        }

        private void OnCloseViewModelEvent(CloseViewModelEvent args)
        {
            if(args.SenderTypeName == nameof(EiaDataSourceViewModel) || args.SenderTypeName == nameof(PiDataSourceViewModel)) this.SelectedDataSource = null;
        }
    }
}