using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DotNetCommon.MVVM;
using DotNetCommon.PersistenceHelpers;
using EIA.Services.Clients;
using EmissionsMonitorModel.DataSources;
using UnifiedDataExplorer.Constants;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ViewModel.Base;
using UnifiedDataExplorer.ViewModel.DataSources;

namespace UnifiedDataExplorer.ViewModel
{
    public class DataExplorerViewModel : RobustViewModelBase
    {
        private readonly DataExplorerHomeViewModel _homeViewModel;

        public DataExplorerViewModel(DataExplorerHomeViewModel homeViewModel, RobustViewModelDependencies facade) : base(facade)
        {
            Children = new ObservableCollection<ViewModelBase>();
            Children.Add(homeViewModel);
            CurrentChild = homeViewModel;
            _homeViewModel = homeViewModel;

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
            if (args.SenderTypeName == nameof(ExplorePointViewModel))
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
                var datasetFinder = this.Resolve<EiaDatasetFinderViewModel>();
                AddAndSwitchChild(datasetFinder);
                await datasetFinder.LoadAsync(args.DataSourceId);
            }
            if (args.SenderTypeName == nameof(EiaDatasetFinderViewModel))
            {
                EiaSeriesViewModel vm = this.Resolve<EiaSeriesViewModel>();
                Logger.LogInformation($"Loading series {args.Id}");
                AddAndSwitchChild(vm);
                await vm.LoadAsync(args);
            }

            //PI
            if (args.SenderTypeName == nameof(PiDataSourceViewModel))
            {
                if (args.Verb == PiDataSourceViewModel.OPEN_AF_EXPLORER)
                {
                    PiDatasetFinderViewModel vm = this.Resolve<PiDatasetFinderViewModel>();
                    AddAndSwitchChild(vm);
                    await vm.LoadAsync(args.DataSourceId);
                }

                if (args.Verb == PiDataSourceViewModel.OPEN_PI_SEARCH_EXPLORER)
                {
                    PiSearchViewModel vm = this.Resolve<PiSearchViewModel>();
                    AddAndSwitchChild(vm);
                    await vm.LoadAsync(args.DataSourceId);
                }
            }

            if (args.SenderTypeName == nameof(PiDatasetFinderViewModel))
            {
                if (args.Verb == PiDatasetFinderViewModel.SHOW_JSON)
                {
                    PiJsonDisplayViewModel vm = this.Resolve<PiJsonDisplayViewModel>();
                    AddAndSwitchChild(vm);
                    await vm.LoadAsync(args);
                }
                if (args.Verb == PiDatasetFinderViewModel.RENDER_VALUES)
                {
                    PiAssetValuesViewModel vm = this.Resolve<PiAssetValuesViewModel>();
                    AddAndSwitchChild(vm);
                    await vm.LoadAsync(args);
                }
            }

            if (args.SenderTypeName == nameof(PiAssetValuesViewModel))
            {
                if (args.Verb == PiAssetValuesViewModel.SHOW_DETAILS_AS_JSON)
                {
                    PiJsonDisplayViewModel vm = this.Resolve<PiJsonDisplayViewModel>();
                    AddAndSwitchChild(vm);
                    await vm.LoadAsync(args);
                }
                if (args.Verb == PiAssetValuesViewModel.RENDER_INTERPOLATED_VALUES)
                {
                    PiInterpolatedDataViewModel vm = this.Resolve<PiInterpolatedDataViewModel>();
                    AddAndSwitchChild(vm);
                    await vm.LoadAsync(args);
                }
            }
            if (args.SenderTypeName == nameof(PiSearchViewModel))
            {
                if (args.Verb == PiSearchViewModel.SHOW_JSON)
                {
                    PiJsonDisplayViewModel vm = this.Resolve<PiJsonDisplayViewModel>();
                    AddAndSwitchChild(vm);
                    await vm.LoadAsync(args);
                }
                if (args.Verb == PiSearchViewModel.RENDER_VALUES)
                {
                    PiInterpolatedDataViewModel vm = this.Resolve<PiInterpolatedDataViewModel>();
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