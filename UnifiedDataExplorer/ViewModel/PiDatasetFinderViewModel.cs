using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.MVVM;
using EmissionsMonitorServices.DataSourceWrappers;
using PiModel;
using PiServices;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ModelWrappers;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel
{
    public class PiDatasetFinderViewModel : RobustViewModelBase
    {
        private readonly DataSourceServiceFactory _clientFactory;
        private int _dataSourceId;
        private PiHttpClient _client;
        public const string SHOW_JSON = "SHOW_JSON";
        public const string RENDER_VALUES = "RENDER_VALUES";

        public PiDatasetFinderViewModel(DataSourceServiceFactory clientFactory, RobustViewModelDependencies facade) : base(facade)
        {
            _clientFactory = clientFactory;
            _categories = new ObservableCollection<LazyTreeItemViewModel>();

            ViewJsonCommand = new DelegateCommand<LazyTreeItemViewModel>(OnViewJson);
            RenderValuesCommand = new DelegateCommand<LazyTreeItemViewModel>(OnRenderValues);
        }

        public string Header => "PI Asset Framework Finder";
        public string HeaderDetail => "Navigate to a PI AssetFramework (AF) dataset";
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

        public ICommand ViewJsonCommand { get; }
        public ICommand RenderValuesCommand { get; }

        public async Task LoadAsync(int dataSourceId)
        {
            _dataSourceId = dataSourceId;
            _client = _clientFactory.GetDataSourceServiceById<PiHttpClient>(dataSourceId);
            AssetServer root = await _client.AssetServerSearch(_client.DefaultAssetServer);
            ServerDatabaseAssetWrapper wrapper = new ServerDatabaseAssetWrapper(root);
            Categories.Add(new LazyTreeItemViewModel(wrapper));
        }

        public async Task LoadChildrenAsync(LazyTreeItemViewModel treeItem)
        {
            if (!treeItem.ChildrenLoaded)
            {
                ILazyTreeItemBackingModel modelInterface = treeItem.GetBackingModel();
                ServerDatabaseAssetWrapper model = modelInterface as ServerDatabaseAssetWrapper;

                if (model != null && model.IsAssetServer())
                {
                    IEnumerable<Database> databases = await _client.GetAssetServerDatabases(model.GetItemName());
                    foreach (Database database in databases) { model.Children.Add(new ServerDatabaseAssetWrapper(database)); };
                    treeItem.AppendLoadedChildren(model.Children);
                }
                else if (model != null && model.IsDatabase())
                {
                    IEnumerable<Asset> assets = await _client.GetDatabaseAssets(model.AsDatabase());
                    foreach (Asset asset in assets) { model.Children.Add(new ServerDatabaseAssetWrapper(asset)); };
                    treeItem.AppendLoadedChildren(model.Children);
                }
                else if (model != null && model.IsAsset())
                {
                    IEnumerable<Asset> assets = await _client.GetChildAssets(model.AsAsset());
                    foreach (Asset asset in assets) { model.Children.Add(new ServerDatabaseAssetWrapper(asset)); };
                    treeItem.AppendLoadedChildren(model.Children);
                }
            }
        }

        public async Task PeformLeafActionAsync(LazyTreeItemViewModel treeItem)
        {//This gets executed when the tree item is double clicked. Because we present different options with buttons this isn't being used
        }

        public void OnViewJson(LazyTreeItemViewModel treeItem)
        {
            ILazyTreeItemBackingModel modelInterface = treeItem.GetBackingModel();
            ServerDatabaseAssetWrapper model = modelInterface as ServerDatabaseAssetWrapper;
            if (model != null)
            {
                PublishOpenViewModelEvent(model, SHOW_JSON);
            }
        }

        private void OnRenderValues(LazyTreeItemViewModel treeItem)
        {
            ILazyTreeItemBackingModel modelInterface = treeItem.GetBackingModel();
            ServerDatabaseAssetWrapper model = modelInterface as ServerDatabaseAssetWrapper;
            if (model != null && model.IsAsset())
            {
                PublishOpenViewModelEvent(model, RENDER_VALUES);
            }
        }

        private void PublishOpenViewModelEvent(ServerDatabaseAssetWrapper model, string verb)
        {
            this.MessageHub.Publish<OpenDataSourceViewModelEvent>(new OpenDataSourceViewModelEvent()
            {
                Sender = this,
                SenderTypeName = nameof(PiDatasetFinderViewModel),
                DataSourceId = _dataSourceId,
                Id = model.GetLinkToSelf(),
                Name = model.GetItemName(),
                Verb = verb,
                TypeTag = model.GetTypeTag()
            });
        }
    }
}