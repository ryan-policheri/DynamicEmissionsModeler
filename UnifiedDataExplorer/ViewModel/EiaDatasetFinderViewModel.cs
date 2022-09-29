using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DotNetCommon.MVVM;
using EIA.Domain.Constants;
using EIA.Domain.Model;
using EIA.Services.Clients;
using EmissionsMonitorServices.DataSourceWrappers;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ModelWrappers;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel
{
    public class EiaDatasetFinderViewModel : RobustViewModelBase
    {
        private readonly DataSourceServiceFactory _clientFactory;
        private int _dataSourceId;
        private EiaClient _client;

        public EiaDatasetFinderViewModel(DataSourceServiceFactory clientFactory, RobustViewModelDependencies facade) : base(facade)
        {
            _clientFactory = clientFactory;
            _categories = new ObservableCollection<LazyTreeItemViewModel>();
        }

        public string Header => "EIA Dataset Finder";
        public string HeaderDetail => "Navigate to a EIA dataset";
        public bool IsCloseable => false;

        private ObservableCollection<LazyTreeItemViewModel> _categories;
        public ObservableCollection<LazyTreeItemViewModel> Categories
        {
            get { return _categories; }
            private set
            {
                _categories = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadAsync(int dataSourceId)
        {
            _dataSourceId = dataSourceId;
            _client = _clientFactory.GetDataSourceServiceById<EiaClient>(dataSourceId);
            Category root = await _client.GetCategoryByIdAsync(EiaCategories.ABSOLUTE_ROOT);
            CategorySeriesWrapper wrapper = new CategorySeriesWrapper(root);
            Categories.Add(new LazyTreeItemViewModel(wrapper));
        }

        public async Task LoadChildrenAsync(LazyTreeItemViewModel treeItem)
        {
            if (!treeItem.ChildrenLoaded)
            {
                ILazyTreeItemBackingModel modelInterface = treeItem.GetBackingModel();
                CategorySeriesWrapper model = modelInterface as CategorySeriesWrapper;

                if(model != null && model.IsCategory())
                {
                    Category itemToLoad = await _client.GetCategoryByIdAsync(int.Parse(treeItem.Id));
                    CategorySeriesWrapper wrappedItem = new CategorySeriesWrapper(itemToLoad);
                    treeItem.AppendLoadedChildren(wrappedItem.Children);
                }
            }
        }

        public void PeformLeafActionAsync(LazyTreeItemViewModel treeItem)
        {
            ILazyTreeItemBackingModel modelInterface = treeItem.GetBackingModel();
            CategorySeriesWrapper model = modelInterface as CategorySeriesWrapper;
            if (model != null && model.IsSeries())
            {
                this.MessageHub.Publish<OpenDataSourceViewModelEvent>(new OpenDataSourceViewModelEvent
                {
                    Sender = this,
                    SenderTypeName = nameof(EiaDatasetFinderViewModel),
                    DataSourceId = _dataSourceId,
                    Id = model.GetId()
                });
            }
        }
    }
}