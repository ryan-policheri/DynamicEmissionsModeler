using System.Collections.ObjectModel;
using System.Windows.Input;
using DotNetCommon.MVVM;

namespace EIADataViewer.ViewModel.MainMenu
{
    public class MenuItemViewModel : ViewModelBase
    {
        public MenuItemViewModel(string header, ICommand command, MenuItemViewModel parent)
        {
            Header = header;
            Command = command;
            Parent = parent;
            Children = new ObservableCollection<MenuItemViewModel>();
        }

        public string Header { get; }

        public ICommand Command { get; }

        public MenuItemViewModel Parent { get; }

        public ObservableCollection<MenuItemViewModel> Children { get; }
    }
}
