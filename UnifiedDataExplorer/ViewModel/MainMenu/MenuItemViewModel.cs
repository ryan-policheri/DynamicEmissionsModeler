using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.MVVM;
using DotNetCommon.PersistenceHelpers;

namespace UnifiedDataExplorer.ViewModel.MainMenu
{
    public class MenuItemViewModel : ViewModelBase
    {
        private object _innerCommand;
        private object _commandArgument;

        public MenuItemViewModel(string header, ICommand innerCommand, MenuItemViewModel parent) : this(header, innerCommand, parent, null)
        {
        }

        public MenuItemViewModel(string header, ICommand innerCommand, MenuItemViewModel parent, object commandArgument)
        {
            _innerCommand = innerCommand;
            _commandArgument = commandArgument;

            Header = header;
            Command = new DelegateCommand(OnOuterCommand);
            Parent = parent;
            Children = new ObservableCollection<MenuItemViewModel>();
        }

        public string Header { get; }

        public ICommand Command { get; }

        public MenuItemViewModel Parent { get; }

        public ObservableCollection<MenuItemViewModel> Children { get; }


        private void OnOuterCommand()
        {
            if(_innerCommand is DelegateCommand)
            {
                (_innerCommand as DelegateCommand).Execute();
            }
            else if(_innerCommand is DelegateCommand<AppDataFile>) //TODO: Fix this. This class shouldn't have to know the type of the command argument. Should all get configured in MainMenuViewModel --> BuildMenuItems
            {
                (_innerCommand as DelegateCommand<AppDataFile>).Execute(_commandArgument as AppDataFile);
            }
        }
    }
}
