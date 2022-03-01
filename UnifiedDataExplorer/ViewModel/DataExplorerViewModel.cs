using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DotNetCommon.MVVM;
using UnifiedDataExplorer.Constants;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel
{
    public class DataExplorerViewModel : RobustViewModelBase
    {
        private readonly EiaDatasetFinderViewModel _eiaDatasetFinderViewModel;
        private readonly PiDatasetFinderViewModel _piDatasetFinderViewModel;

        public DataExplorerViewModel(EiaDatasetFinderViewModel datasetFinderViewModel,
            PiDatasetFinderViewModel piDatasetFinderViewModel, 
            RobustViewModelDependencies facade) : base(facade)
        {
            _eiaDatasetFinderViewModel = datasetFinderViewModel;
            _piDatasetFinderViewModel = piDatasetFinderViewModel;

            CurrentChild = datasetFinderViewModel;
            Children = new ObservableCollection<ViewModelBase>();

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
            Children.Add(_eiaDatasetFinderViewModel);
            CurrentChild = _eiaDatasetFinderViewModel;
            await _eiaDatasetFinderViewModel.LoadAsync();

            Children.Add(_piDatasetFinderViewModel);
            CurrentChild = _piDatasetFinderViewModel;
            await _piDatasetFinderViewModel.LoadAsync();
        }

        private void OnCloseViewModel(CloseViewModelEvent args)
        {
            if (args.SenderTypeName == nameof(SeriesViewModel))
            {
                CurrentChild = _eiaDatasetFinderViewModel;
                this.Children.Remove(args.Sender as ViewModelBase);
            }
            if (args.SenderTypeName == nameof(JsonDisplayViewModel))
            {
                CurrentChild = _piDatasetFinderViewModel;
                this.Children.Remove(args.Sender as ViewModelBase);
            }
        }

        private async void OnOpenViewModel(OpenViewModelEvent args)
        {
            if (args.SenderTypeName == nameof(EiaDatasetFinderViewModel))
            {
                SeriesViewModel vm = this.Resolve<SeriesViewModel>();
                Logger.LogInformation($"Loading series {args.Id}");
                Children.Add(vm);
                CurrentChild = vm;
                await vm.LoadAsync(args.Id);
            }
            if (args.SenderTypeName == nameof(PiDatasetFinderViewModel))
            {
                Children.Add(args.ViewModel);
                CurrentChild = args.ViewModel;
            }
        }

        private async void OnMenuItemEvent(MenuItemEvent args)
        {
            if(args.MenuItemHeader == MenuItemHeaders.SAVE_OPEN_SERIES)
            {
                ICollection<string> ids = new List<string>();

                foreach (ViewModelBase child in this.Children)
                {
                    if (child.GetType() == typeof(SeriesViewModel))
                    {
                        SeriesViewModel vm = child as SeriesViewModel;
                        ids.Add(vm.SeriesId);
                    }
                }

                throw new NotImplementedException();

                //AppDataFile file = new AppDataFile(ids);
                //try
                //{
                //    await DataFiler.SaveObjectAsFile(file);
                //}
                //catch(Exception ex)
                //{
                //    throw;
                //}
            }
        }
    }
}
