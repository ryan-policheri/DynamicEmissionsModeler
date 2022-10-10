using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.DataSources;
using EmissionsMonitorServices.DataSourceWrappers;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ViewModel.Base;
using UnifiedDataExplorer.ViewModel.DataSources;
using UnifiedDataExplorer.ViewModel.VirtualFileSystem;

namespace UnifiedDataExplorer.ViewModel.DataExploring
{
    public class DataExploringHomeViewModel : RobustViewModelBase
    {
        private readonly IDataSourceRepository _repo;
        private readonly DataSourceServiceFactory _serviceFactory;

        public DataExploringHomeViewModel(IDataSourceRepository repo,
            DataSourceServiceFactory serviceFactory,
            ExploreSetFileSystemViewModel exploreSetFileSysVm,
            RobustViewModelDependencies facade) : base(facade)
        {
            _repo = repo;
            _serviceFactory = serviceFactory;

            DataSources = new ObservableCollection<DataSourceBaseViewModel>();
            AddDataSource = new DelegateCommand(OnAddDataSource);

            ExploreSetFileSysVm = exploreSetFileSysVm;

            this.MessageHub.Subscribe<SaveViewModelEvent>(OnSaveViewModelEvent);
            CloseExplorationItemCommand = new DelegateCommand(OnCloseExplorationItem);
        }

        public string Header => "Home";
        public string HeaderDetail => "Exploration Home";
        public bool IsCloseable => false;

        public ObservableCollection<DataSourceBaseViewModel> DataSources { get; }

        public ICommand AddDataSource { get; }

        public ExploreSetFileSystemViewModel ExploreSetFileSysVm { get; }

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

            await ExploreSetFileSysVm.LoadAsync(FileSystemMode.OpenOnly);
        }

        private async void OnAddDataSource()
        {
            DataSourceManagerViewModel viewModel = this.Resolve<DataSourceManagerViewModel>();
            await viewModel.LoadAsync();
            this.DialogService.ShowModalWindow<DataSourceManagerViewModel>(viewModel);
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
