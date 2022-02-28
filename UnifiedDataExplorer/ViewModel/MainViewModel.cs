using System.Threading.Tasks;
using DotNetCommon.MVVM;
using EIADataViewer.ViewModel.Base;
using EIADataViewer.ViewModel.MainMenu;

namespace EIADataViewer.ViewModel
{
    public class MainViewModel : RobustViewModelBase
    {
        private readonly DataExplorerViewModel _dataExplorer;

        public MainViewModel(DataExplorerViewModel dataExplorer, MainMenuViewModel toolBar, RobustViewModelDependencies facade) : base(facade)
        {
            MainToolBar = toolBar;
            _dataExplorer = dataExplorer;
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
