using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.DataSources;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ViewModel.Base;
using UnifiedDataExplorer.ViewModel.DataSources;

namespace UnifiedDataExplorer.ViewModel
{
    public class DataExplorerHomeViewModel : RobustViewModelBase
    {
        private readonly IDataSourceRepository _repo;

        public DataExplorerHomeViewModel(IDataSourceRepository repo, RobustViewModelDependencies facade) : base(facade)
        {
            _repo = repo;
            DataSources = new ObservableCollection<DataSourceBase>();
            AddDataSource = new DelegateCommand(OnAddDataSource);
            this.MessageHub.Subscribe<SaveViewModelEvent>(OnSaveViewModelEvent);
        }

        public string Header => "Home";
        public string HeaderDetail => "Open an explorer to find data!";
        public bool IsCloseable => false;

        public ObservableCollection<DataSourceBase> DataSources { get; }

        public ICommand AddDataSource { get; }

        public async Task LoadAsync()
        {
            DataSources.Clear();
            foreach(DataSourceBase dataSource in (await _repo.GetAllDataSourcesAsync())) DataSources.Add(dataSource);
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
    }
}
