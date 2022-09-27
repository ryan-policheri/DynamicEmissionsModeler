using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DotNetCommon.MVVM;
using EIA.Domain.Constants;
using EIA.Domain.Model;
using EIA.Services.Clients;
using EmissionsMonitorModel.DataSources;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ModelWrappers;
using UnifiedDataExplorer.ViewModel.Base;
using UnifiedDataExplorer.ViewModel.DataSources;

namespace UnifiedDataExplorer.ViewModel
{
    public class EiaDatasetFinderViewModel : RobustViewModelBase
    {
        private readonly EiaClient _client;

        public EiaDatasetFinderViewModel(EiaClient client, RobustViewModelDependencies facade) : base(facade)
        {
            _client = client;
            _categories = new ObservableCollection<LazyTreeItemViewModel>();
        }

        public string Header => "EIA Dataset Finder";
        public string HeaderDetail => "Navigate to a EIA dataset";
        public bool IsCloseable => false;

        public IEiaConnectionInfo DataSourceConnectionInfo { get; private set; }

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

        public async Task LoadAsync(EiaDataSource dataSource)
        {
            DataSourceConnectionInfo = dataSource;
            _client.Initialize(DataSourceConnectionInfo);
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

        public async Task PeformLeafActionAsync(LazyTreeItemViewModel treeItem)
        {
            ILazyTreeItemBackingModel modelInterface = treeItem.GetBackingModel();
            CategorySeriesWrapper model = modelInterface as CategorySeriesWrapper;
            if (model != null && model.IsSeries())
            {
                this.MessageHub.Publish<OpenViewModelEvent>(new OpenViewModelEvent { Sender = this, SenderTypeName = nameof(EiaDatasetFinderViewModel), Id = model.GetId() });
                return;
            }
        }
    }
}