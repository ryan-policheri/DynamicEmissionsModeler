using System.Collections.Generic;
using System.Collections.ObjectModel;
using DotNetCommon.DelegateCommand;
using DotNetCommon.PersistenceHelpers;
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
            MessageHub.Subscribe<LoadingEvent>(OnLoadingEvent);
        }

        public ObservableCollection<MenuItemViewModel> MenuItems { get; }

        private bool _isLoading;
        public bool IsLoading 
        { 
            get { return _isLoading; }
            set { SetField<bool>(ref _isLoading, value); }
        }

        private void BuildMenuItems()
        {
            MenuItemViewModel file = new MenuItemViewModel("File", null, null);
            MenuItemViewModel saveOpenExplorePoints = new MenuItemViewModel(MenuItemHeaders.SAVE_OPEN_EXPLORE_POINTS, new DelegateCommand(SaveOpenExplorePoints), file);
            file.Children.Add(saveOpenExplorePoints);

            MenuItemViewModel openExplorePoints = new MenuItemViewModel(MenuItemHeaders.OPEN_SAVE, null, file);
            file.Children.Add(openExplorePoints);

            IEnumerable<AppDataFile> appDataFiles = AppDataFile.RetrieveAllAppFilesInDirectory(DataFileProvider.BuildDataViewFile().RootSaveDirectory);
            foreach (AppDataFile appDataFile in appDataFiles)
            {
                MenuItemViewModel vm = new MenuItemViewModel(appDataFile.FileDisplayName, new DelegateCommand<AppDataFile>(OpenSave), openExplorePoints, appDataFile);
                openExplorePoints.Children.Add(vm);
            }

            MenuItemViewModel actions = new MenuItemViewModel("Actions", null, null);
            MenuItemViewModel closeAll = new MenuItemViewModel(MenuItemHeaders.CLOSE_ALL, new DelegateCommand(OnCloseAll), actions);
            actions.Children.Add(closeAll);

            MenuItemViewModel reports = new MenuItemViewModel("Reports", null, null);
            MenuItemViewModel buildingEmissionsReport = new MenuItemViewModel(MenuItemHeaders.RENDER_BUILDING_REPORT, new DelegateCommand(OnBuildingReport), reports);
            reports.Children.Add(buildingEmissionsReport);

            MenuItemViewModel edit = new MenuItemViewModel("Edit", null, null);
            MenuItemViewModel settings = new MenuItemViewModel("Settings", null, null);
            MenuItems.Add(file);
            MenuItems.Add(actions);
            MenuItems.Add(reports);
            MenuItems.Add(edit);
            MenuItems.Add(settings);
        }

        private void SaveOpenExplorePoints()
        {
            MessageHub.Publish<MenuItemEvent>(new MenuItemEvent { Sender = this, SenderTypeName = nameof(MainMenuViewModel), MenuItemHeader = MenuItemHeaders.SAVE_OPEN_EXPLORE_POINTS });
        }

        private void OpenSave(AppDataFile saveFile)
        {
            MessageHub.Publish<MenuItemEvent>(new MenuItemEvent { Sender = this,
                SenderTypeName = nameof(MainMenuViewModel),
                MenuItemHeader = MenuItemHeaders.OPEN_SAVE,
                Action = MenuItemHeaders.OPEN_SAVE,
                Data = saveFile
            });
        }

        private void OnCloseAll()
        {
            MessageHub.Publish<MenuItemEvent>(new MenuItemEvent
            {
                Sender = this,
                SenderTypeName = nameof(MainMenuViewModel),
                MenuItemHeader = MenuItemHeaders.CLOSE_ALL,
                Action = MenuItemHeaders.CLOSE_ALL
            });
        }

        private void OnBuildingReport()
        {
            MessageHub.Publish<MenuItemEvent>(new MenuItemEvent
            {
                Sender = this,
                SenderTypeName = nameof(MainMenuViewModel),
                MenuItemHeader = MenuItemHeaders.RENDER_BUILDING_REPORT,
                Action = MenuItemHeaders.RENDER_BUILDING_REPORT
            });
        }

        private void OnLoadingEvent(LoadingEvent args)
        {
            IsLoading = args.IsLoading;
        }
    }
}
