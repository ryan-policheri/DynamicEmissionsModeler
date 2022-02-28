using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DotNetCommon.MVVM;
using DotNetCommon.PersistenceHelpers;
using EIADataViewer.Constants;
using EIADataViewer.Events;
using EIADataViewer.ViewModel.Base;
using System;

namespace EIADataViewer.ViewModel
{
    public class DataExplorerViewModel : RobustViewModelBase
    {
        private readonly DatasetFinderViewModel _datasetFinderViewModel;

        public DataExplorerViewModel(DatasetFinderViewModel datasetFinderViewModel, RobustViewModelDependencies facade) : base(facade)
        {
            _datasetFinderViewModel = datasetFinderViewModel;
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
            Children.Add(_datasetFinderViewModel);
            CurrentChild = _datasetFinderViewModel;
            await _datasetFinderViewModel.LoadAsync();
        }

        private void OnCloseViewModel(CloseViewModelEvent args)
        {
            if (args.SenderTypeName == nameof(SeriesViewModel))
            {
                CurrentChild = _datasetFinderViewModel;
                this.Children.Remove(args.Sender as ViewModelBase);
            }
        }

        private async void OnOpenViewModel(OpenViewModelEvent args)
        {
            if (args.SenderTypeName == nameof(DatasetFinderViewModel))
            {
                SeriesViewModel vm = this.Resolve<SeriesViewModel>();
                Logger.LogInformation($"Loading series {args.Id}");
                Children.Add(vm);
                CurrentChild = vm;
                await vm.LoadAsync(args.Id);
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

                AppDataFile file = new AppDataFile(ids);
                try
                {
                    await DataFiler.SaveObjectAsFile(file);
                }
                catch(Exception ex)
                {
                    throw;
                }
            }
        }
    }
}
