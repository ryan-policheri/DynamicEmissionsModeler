using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DotNetCommon.Extensions;
using DotNetCommon.MVVM;
using DotNetCommon.PersistenceHelpers;
using Microsoft.Extensions.Logging;
using UnifiedDataExplorer.Constants;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ViewModel.Base;
using UnifiedDataExplorer.ViewModel.DataExploring.ExplorePoints;
using UnifiedDataExplorer.ViewModel.DataExploring.Explorers;
using UnifiedDataExplorer.ViewModel.DataSources;
using UnifiedDataExplorer.ViewModel.VirtualFileSystem;

namespace UnifiedDataExplorer.ViewModel.DataExploring
{
    public class DataExplorationManagerViewModel : RobustViewModelBase
    {
        private readonly DataExploringHomeViewModel _homeViewModel;

        public DataExplorationManagerViewModel(DataExploringHomeViewModel homeViewModel, ExploreSetFileSystemViewModel navigationVm, RobustViewModelDependencies facade) : base(facade)
        {
            _homeViewModel = homeViewModel;
            Children = new ObservableCollection<ViewModelBase>();
            Children.Add(homeViewModel);
            CurrentChild = homeViewModel;

            MessageHub.Subscribe<OpenDataSourceViewModelEvent>(OpenDataSourceViewModelEvent);
            MessageHub.Subscribe<CloseViewModelEvent>(OnCloseViewModel);
            MessageHub.Subscribe<MenuItemEvent>(OnMenuItemEvent);
        }

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

        public ObservableCollection<ViewModelBase> Children { get; }


        public async Task LoadAsync()
        {
            await _homeViewModel.LoadAsync();
        }

        private void AddAndSwitchChild(ViewModelBase viewModel)
        {
            Children.Add(viewModel);
            CurrentChild = viewModel;
        }

        private void OnCloseViewModel(CloseViewModelEvent args)
        {
            if (args.SenderTypeName.OneOf(new string[] { nameof(DataExploringHomeViewModel), nameof(ExplorerViewModel), nameof(ExplorePointViewModel) }) )
            {
                int indexBefore = this.Children.IndexOf(args.Sender as ViewModelBase) - 1;
                ViewModelBase childBefore = this.Children[indexBefore];
                this.Children.Remove(args.Sender as ViewModelBase);
                this.CurrentChild = childBefore;
            }
        }

        private async void OpenDataSourceViewModelEvent(OpenDataSourceViewModelEvent args)
        {
            //EIA
            if (args.SenderTypeName == nameof(EiaDataSourceViewModel) && args.Verb == EiaDataSourceViewModel.OPEN_EIA_EXPLORER)
            {
                var datasetFinder = this.Resolve<EiaApiExplorerViewModel>();
                AddAndSwitchChild(datasetFinder);
                await datasetFinder.LoadAsync(args.DataSourceId);
            }
            if (args.SenderTypeName == nameof(EiaApiExplorerViewModel))
            {
                EiaSeriesExplorePointViewModel vm = this.Resolve<EiaSeriesExplorePointViewModel>();
                Logger.LogInformation($"Loading series {args.Id}");
                AddAndSwitchChild(vm);
                await vm.LoadAsync(args);
            }

            //PI
            if (args.SenderTypeName == nameof(PiDataSourceViewModel))
            {
                if (args.Verb == PiDataSourceViewModel.OPEN_AF_EXPLORER)
                {
                    PiAssetFrameworkExplorerViewModel vm = this.Resolve<PiAssetFrameworkExplorerViewModel>();
                    AddAndSwitchChild(vm);
                    await vm.LoadAsync(args.DataSourceId);
                }

                if (args.Verb == PiDataSourceViewModel.OPEN_PI_SEARCH_EXPLORER)
                {
                    PiTagExplorerViewModel vm = this.Resolve<PiTagExplorerViewModel>();
                    AddAndSwitchChild(vm);
                    await vm.LoadAsync(args.DataSourceId);
                }
            }

            if (args.SenderTypeName == nameof(PiAssetFrameworkExplorerViewModel))
            {
                if (args.Verb == PiAssetFrameworkExplorerViewModel.SHOW_JSON)
                {
                    PiJsonDisplayExplorePointViewModel vm = this.Resolve<PiJsonDisplayExplorePointViewModel>();
                    AddAndSwitchChild(vm);
                    await vm.LoadAsync(args);
                }
                if (args.Verb == PiAssetFrameworkExplorerViewModel.RENDER_VALUES)
                {
                    PiAssetValuesExplorePointViewModel vm = this.Resolve<PiAssetValuesExplorePointViewModel>();
                    AddAndSwitchChild(vm);
                    await vm.LoadAsync(args);
                }
            }

            if (args.SenderTypeName == nameof(PiAssetValuesExplorePointViewModel))
            {
                if (args.Verb == PiAssetValuesExplorePointViewModel.SHOW_DETAILS_AS_JSON)
                {
                    PiJsonDisplayExplorePointViewModel vm = this.Resolve<PiJsonDisplayExplorePointViewModel>();
                    AddAndSwitchChild(vm);
                    await vm.LoadAsync(args);
                }
                if (args.Verb == PiAssetValuesExplorePointViewModel.RENDER_INTERPOLATED_VALUES)
                {
                    PiInterpolatedDataExplorePointViewModel vm = this.Resolve<PiInterpolatedDataExplorePointViewModel>();
                    AddAndSwitchChild(vm);
                    await vm.LoadAsync(args);
                }
            }
            if (args.SenderTypeName == nameof(PiTagExplorerViewModel))
            {
                if (args.Verb == PiTagExplorerViewModel.SHOW_JSON)
                {
                    PiJsonDisplayExplorePointViewModel vm = this.Resolve<PiJsonDisplayExplorePointViewModel>();
                    AddAndSwitchChild(vm);
                    await vm.LoadAsync(args);
                }
                if (args.Verb == PiTagExplorerViewModel.RENDER_VALUES)
                {
                    PiInterpolatedDataExplorePointViewModel vm = this.Resolve<PiInterpolatedDataExplorePointViewModel>();
                    AddAndSwitchChild(vm);
                    await vm.LoadAsync(args);
                }
            }
        }

        private void OnMenuItemEvent(MenuItemEvent args)
        {
            if (args.MenuItemHeader == MenuItemHeaders.SAVE_OPEN_EXPLORE_POINTS)
            {
                List<OpenDataSourceViewModelEvent> openingEvents = new List<OpenDataSourceViewModelEvent>();

                foreach (ViewModelBase child in this.Children)
                {
                    if (child is ExplorePointViewModel)
                    {
                        openingEvents.Add(((ExplorePointViewModel)child).CurrentLoadingInfo as OpenDataSourceViewModelEvent);
                    }
                }

                
                FileSaveSettingsViewModel vm = new FileSaveSettingsViewModel();
                while (vm.SaveName == null) { DialogService.ShowModalWindow(vm); }
                
                AppDataFile file = DataFileProvider.BuildDataViewFile();
                file.Save<List<OpenDataSourceViewModelEvent>>(openingEvents, vm.SaveName);
            }
            else if (args.MenuItemHeader == MenuItemHeaders.OPEN_SAVE)
            {
                AppDataFile file = args.Data as AppDataFile;
                if (file != null)
                {
                    List<OpenDataSourceViewModelEvent> openingEvents = file.Read<List<OpenDataSourceViewModelEvent>>();
                    foreach (OpenDataSourceViewModelEvent openEvent in openingEvents)
                    {
                        this.OpenDataSourceViewModelEvent(openEvent);
                    }
                }
            }
            else if (args.MenuItemHeader == MenuItemHeaders.CLOSE_ALL)
            {
                for (int i = Children.Count - 1; i >= 0; i--)
                {
                    ViewModelBase child = Children[i];
                    if (child is ExplorePointViewModel)
                    {
                        Children.RemoveAt(i);
                    }
                }
            }
        }
    }
}