using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.MVVM;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.DataSources;
using EmissionsMonitorServices.DataSourceWrappers;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ViewModel.Base;
using UnifiedDataExplorer.ViewModel.DataSources;
using UnifiedDataExplorer.ViewModel.ProcessModeling;
using UnifiedDataExplorer.ViewModel.VirtualFileSystem;

namespace UnifiedDataExplorer.ViewModel.DataExploring
{
    public class DataExploringHomeViewModel : RobustViewModelBase
    {
        private const string UNSELECTED = "(Select Option)";
        private const string EXPLORE_SET = "Explore Set";
        private const string ENERGY_MODEL = "Energy Model";

        private readonly IDataSourceRepository _repo;
        private readonly DataSourceServiceFactory _serviceFactory;
        private readonly ExploreSetFileSystemViewModel _exploreSetFileSysVm;
        private readonly EnergyModelFileSystemViewModel _energyModelFileSysVm;

        public DataExploringHomeViewModel(IDataSourceRepository repo,
            DataSourceServiceFactory serviceFactory,
            ExploreSetFileSystemViewModel exploreSetFileSysVm,
            EnergyModelFileSystemViewModel energyModelFileSysVm,
            RobustViewModelDependencies facade) : base(facade)
        {
            _repo = repo;
            _serviceFactory = serviceFactory;

            DataSources = new ObservableCollection<DataSourceBaseViewModel>();
            AddDataSource = new DelegateCommand(OnAddDataSource);
            CreateEnergyModel = new DelegateCommand(OnCreateEnergyModel);
            OpenOptions = new List<string>() { UNSELECTED, EXPLORE_SET, ENERGY_MODEL };
            SelectedOpenOption = OpenOptions.First();

            _exploreSetFileSysVm = exploreSetFileSysVm;
            _energyModelFileSysVm = energyModelFileSysVm;

            this.MessageHub.Subscribe<SaveViewModelEvent>(OnSaveViewModelEvent);
            CloseExplorationItemCommand = new DelegateCommand(OnCloseExplorationItem);
        }

        public string Header => "Home";
        public string HeaderDetail => "Exploration Home";
        public bool IsCloseable => false;

        public ObservableCollection<DataSourceBaseViewModel> DataSources { get; }

        public ICommand AddDataSource { get; }
        public ICommand CreateEnergyModel { get; }


        private string _selectedOpenOption;
        public string SelectedOpenOption
        {
            get { return _selectedOpenOption; }
            set
            {
                SetField(ref _selectedOpenOption, value);
                if (value == null || value == UNSELECTED) CurrentOpenOptionViewModel = null;
                if (value == EXPLORE_SET) CurrentOpenOptionViewModel = _exploreSetFileSysVm;
                if (value == ENERGY_MODEL) CurrentOpenOptionViewModel = _energyModelFileSysVm;
                OnPropertyChanged(nameof(CurrentOpenOptionViewModel));
            }
        }
        public List<string> OpenOptions { get; }
        public ViewModelBase CurrentOpenOptionViewModel { get; private set; }

        public async Task LoadAsync()
        {//TODO: This is smelly
            await _serviceFactory.LoadAllDataSourceServices();
            DataSourceBaseViewModel untypedVm = this.Resolve<DataSourceBaseViewModel>();
            DataSources.Clear();
            foreach (DataSourceBase dataSource in (await _repo.GetAllDataSourcesAsync()))
            {
                var typedVm = untypedVm.InitializeSubclassViewModel(dataSource.SourceType, dataSource);
                DataSources.Add(typedVm);
            }

            await _exploreSetFileSysVm.LoadAsync(FileSystemMode.OpenOnly);
            await _energyModelFileSysVm.LoadAsync(FileSystemMode.OpenOnly);
        }

        private async void OnAddDataSource()
        {
            DataSourceManagerViewModel viewModel = this.Resolve<DataSourceManagerViewModel>();
            await viewModel.LoadAsync();
            this.DialogService.ShowModalWindow<DataSourceManagerViewModel>(viewModel);
        }

        private void OnCreateEnergyModel()
        {
            ProcessModelMainViewModel viewModel = this.Resolve<ProcessModelMainViewModel>();
            this.DialogService.OpenSecondaryWindow<ProcessModelMainViewModel>(viewModel);
        }

        private async void OnSaveViewModelEvent(SaveViewModelEvent args)
        {
            if (args.SenderTypeName == nameof(EiaDataSourceViewModel) || args.SenderTypeName == nameof(PiDataSourceViewModel))
            {
                await this.LoadAsync();
            }
        }

        public ICommand CloseExplorationItemCommand { get; }

        private void OnCloseExplorationItem()
        {
            MessageHub.Publish(new CloseViewModelEvent { Sender = this, SenderTypeName = nameof(DataExploringHomeViewModel) });
        }

    }
}
