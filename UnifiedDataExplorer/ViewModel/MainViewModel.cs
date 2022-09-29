using System.Threading.Tasks;
using DotNetCommon.MVVM;
using UnifiedDataExplorer.Services.Reporting;
using UnifiedDataExplorer.ViewModel.Base;
using UnifiedDataExplorer.ViewModel.DataExploring;
using UnifiedDataExplorer.ViewModel.MainMenu;

namespace UnifiedDataExplorer.ViewModel
{
    public class MainViewModel : RobustViewModelBase
    {
        private readonly DataExplorationManagerViewModel _dataExplorationManager;
        private readonly ReportProcessor _reportProcessor;

        public MainViewModel(DataExplorationManagerViewModel dataExplorationManager, MainMenuViewModel toolBar, ReportProcessor reportProcessor, RobustViewModelDependencies facade) : base(facade)
        {
            _dataExplorationManager = dataExplorationManager;
            _reportProcessor = reportProcessor;

            MainToolBar = toolBar;
        }

        public MainMenuViewModel MainToolBar { get; }

        private ViewModelBase _currentChild;
        public ViewModelBase CurrentChild 
        {
            get { return _currentChild; }
            set
            {
                _currentChild = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadAsync()
        {
            CurrentChild = _dataExplorationManager;
            await _dataExplorationManager.LoadAsync();
        }
    }
}
