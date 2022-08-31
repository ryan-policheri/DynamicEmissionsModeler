using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DotNetCommon.MVVM;
using DotNetCommon.PersistenceHelpers;
using UnifiedDataExplorer.Constants;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel
{
    public class DataExplorerViewModel : RobustViewModelBase
    {
        public DataExplorerViewModel(DataExplorerHomeViewModel homeViewModel, RobustViewModelDependencies facade) : base(facade)
        {
            Children = new ObservableCollection<ViewModelBase>();
            Children.Add(homeViewModel);
            CurrentChild = homeViewModel;

            MessageHub.Subscribe<OpenViewModelEvent>(OnOpenViewModel);
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

        private async void OnOpenViewModel(OpenViewModelEvent args)
        {
            if (args.SenderTypeName == nameof(EiaDatasetFinderViewModel))
            {
                EiaSeriesViewModel vm = this.Resolve<EiaSeriesViewModel>();
                Logger.LogInformation($"Loading series {args.Id}");
                AddAndSwitchChild(vm);
                await vm.LoadAsync(args);
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
            if(args.SenderTypeName == nameof(PiAssetValuesViewModel))
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
                List<OpenViewModelEvent> openingEvents = new List<OpenViewModelEvent>();

                foreach (ViewModelBase child in this.Children)
                {
                    if (child is ExplorePointViewModel)
                    {
                        openingEvents.Add(((ExplorePointViewModel)child).CurrentLoadingInfo as OpenViewModelEvent);
                    }
                }

                FileSaveSettingsViewModel vm = new FileSaveSettingsViewModel();
                while (vm.SaveName == null) { DialogService.ShowModalWindow(vm); }
                
                AppDataFile file = DataFileProvider.BuildDataViewFile();
                file.Save<List<OpenViewModelEvent>>(openingEvents, vm.SaveName);
            }
            else if (args.MenuItemHeader == MenuItemHeaders.OPEN_SAVE)
            {
                AppDataFile file = args.Data as AppDataFile;
                if (file != null)
                {
                    List<OpenViewModelEvent> openingEvents = file.Read<List<OpenViewModelEvent>>();
                    foreach (OpenViewModelEvent openEvent in openingEvents)
                    {
                        this.OnOpenViewModel(openEvent);
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