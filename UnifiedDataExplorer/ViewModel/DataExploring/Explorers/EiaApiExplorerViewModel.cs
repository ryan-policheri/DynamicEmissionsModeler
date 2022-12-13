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

namespace UnifiedDataExplorer.ViewModel.DataExploring.Explorers
{
    public class EiaApiExplorerViewModel : ExplorerViewModel
    {
        private readonly DataSourceServiceFactory _clientFactory;
        private int _dataSourceId;
        private EiaClient _client;

        public EiaApiExplorerViewModel(DataSourceServiceFactory clientFactory, RobustViewModelDependencies facade) : base(facade)
        {
            _clientFactory = clientFactory;
            _categories = new ObservableCollection<LazyTreeItemViewModel>();
        }

        public override string Header => "EIA Api Explorer";
        public override string HeaderDetail => "Navigate to a EIA dataset";

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

                if (model != null && model.IsCategory())
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
                MessageHub.Publish(new OpenDataSourceViewModelEvent
                {
                    Sender = this,
                    SenderTypeName = nameof(EiaApiExplorerViewModel),
                    DataSourceId = _dataSourceId,
                    Id = model.GetId()
                });
            }
        }
    }
}