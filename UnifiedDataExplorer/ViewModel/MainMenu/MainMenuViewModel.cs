using System.Collections.ObjectModel;
using DotNetCommon.DelegateCommand;
using UnifiedDataExplorer.Constants;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.MainMenu
{
    public class MainMenuViewModel : RobustViewModelBase
    {
        public MainMenuViewModel(RobustViewModelDependencies facade) : base(facade)
        {
            MenuItems = new ObservableCollection<MenuItemViewModel>();
            BuildMenuItems();
        }

        public ObservableCollection<MenuItemViewModel> MenuItems { get; }

        private void BuildMenuItems()
        {
            MenuItemViewModel file = new MenuItemViewModel("File", null, null);
            MenuItemViewModel saveOpenSeries = new MenuItemViewModel(MenuItemHeaders.SAVE_OPEN_EXPLORE_POINTS, new DelegateCommand(SaveOpenSeries), file);
            file.Children.Add(saveOpenSeries);

            MenuItemViewModel edit = new MenuItemViewModel("Edit", null, null);
            MenuItemViewModel settings = new MenuItemViewModel("Settings", null, null);
            MenuItems.Add(file);
            MenuItems.Add(edit);
            MenuItems.Add(settings);
        }

        private void SaveOpenSeries()
        {
            MessageHub.Publish<MenuItemEvent>(new MenuItemEvent { Sender = this, SenderTypeName = nameof(MainMenuViewModel), MenuItemHeader = MenuItemHeaders.SAVE_OPEN_EXPLORE_POINTS });
        }
    }
}
