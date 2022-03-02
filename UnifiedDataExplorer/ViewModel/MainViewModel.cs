using System.Threading.Tasks;
using DotNetCommon.MVVM;
using UnifiedDataExplorer.Services.Reporting;
using UnifiedDataExplorer.ViewModel.Base;
using UnifiedDataExplorer.ViewModel.MainMenu;

namespace UnifiedDataExplorer.ViewModel
{
    public class MainViewModel : RobustViewModelBase
    {
        private readonly DataExplorerViewModel _dataExplorer;
        private readonly ReportProcessor _reportProcessor;

        public MainViewModel(DataExplorerViewModel dataExplorer, MainMenuViewModel toolBar, ReportProcessor reportProcessor, RobustViewModelDependencies facade) : base(facade)
        {
            _dataExplorer = dataExplorer;
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
            CurrentChild = _dataExplorer;
            await _dataExplorer.LoadAsync();
        }
    }
}
