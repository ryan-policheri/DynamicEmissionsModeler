using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.Extensions;
using DotNetCommon.MVVM;
using PiModel;
using PiServices;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ModelWrappers;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel
{
    public class PiDatasetFinderViewModel : RobustViewModelBase
    {
        private readonly PiHttpClient _client;

        public const string SHOW_JSON = "SHOW_JSON";
        public const string RENDER_VALUES = "RENDER_VALUES";

        public PiDatasetFinderViewModel(PiHttpClient client, RobustViewModelDependencies facade) : base(facade)
        {
            _client = client;
            _categories = new ObservableCollection<LazyTreeItemViewModel>();

            ViewJsonCommand = new DelegateCommand<LazyTreeItemViewModel>(OnViewJson);
            RenderValuesCommand = new DelegateCommand<LazyTreeItemViewModel>(OnRenderValues);
        }

        public string Header => "PI Dataset Finder";
        public string HeaderDetail => "Navigate to a PI dataset";
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

        public async Task LoadAsync()
        {
            while (!_client.HasAuthorization)
            {
                PiSettingsViewModel viewModel = new PiSettingsViewModel { PiWebApiUrl = _client.BaseAddress };
                this.DialogService.ShowModalWindow(viewModel);
                _client.UserName = viewModel.PiUserName;
                _client.Password = viewModel.PiPassword;
                _client.AddAuthorizationHeader();
                DataFileProvider.BuildCredentialsFile().Update<CredentialConfig>(x => {
                    x.EncryptedPiUserName = viewModel.PiUserName;
                    x.EncryptedPiPassword = viewModel.PiPassword;
                });
            }
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
        {
            //ILazyTreeItemBackingModel modelInterface = treeItem.GetBackingModel();
            //CategorySeriesWrapper model = modelInterface as CategorySeriesWrapper;
            //if (model != null && model.IsSeries())
            //{
            //    this.MessageHub.Publish<OpenViewModelEvent>(new OpenViewModelEvent { Sender = this, SenderTypeName = nameof(DatasetFinderViewModel), Id = model.GetId() });
            //    return;
            //}
        }

        public void OnViewJson(LazyTreeItemViewModel treeItem)
        {
            ILazyTreeItemBackingModel modelInterface = treeItem.GetBackingModel();
            ServerDatabaseAssetWrapper model = modelInterface as ServerDatabaseAssetWrapper;
            if (model != null)
            {
                this.MessageHub.Publish<OpenViewModelEvent>(new OpenViewModelEvent {
                    Sender = this,
                    SenderTypeName = nameof(PiDatasetFinderViewModel),
                    Id = model.GetLinkToSelf(),
                    Name = model.GetItemName(),
                    Verb = SHOW_JSON,
                    Tag = model.GetTypeTag(),
                });
            }
        }


        private async void OnRenderValues(LazyTreeItemViewModel treeItem)
        {
            ILazyTreeItemBackingModel modelInterface = treeItem.GetBackingModel();
            ServerDatabaseAssetWrapper model = modelInterface as ServerDatabaseAssetWrapper;
            if (model != null && model.IsAsset())
            {
                Asset asset = model.AsAsset();
                await _client.LoadAssetValues(asset);

                this.MessageHub.Publish<OpenViewModelEvent>(new OpenViewModelEvent { 
                    Sender = this, 
                    SenderTypeName = nameof(PiDatasetFinderViewModel), 
                    Id = model.GetLinkToSelf(),
                    Name = model.GetItemName(),
                    Verb = RENDER_VALUES,
                    Tag = model.GetTypeTag()
                });
            }
        }
    }
}