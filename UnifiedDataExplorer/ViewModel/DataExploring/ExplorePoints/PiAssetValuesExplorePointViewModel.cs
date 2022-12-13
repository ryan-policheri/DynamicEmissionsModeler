using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.EventAggregation;
using DotNetCommon.Extensions;
using EmissionsMonitorServices.DataSourceWrappers;
using PiModel;
using PiServices;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ModelWrappers;

namespace UnifiedDataExplorer.ViewModel.DataExploring.ExplorePoints
{
    public class PiAssetValuesExplorePointViewModel : ExplorePointViewModel
    {
        private readonly DataSourceServiceFactory _clientFactory;
        private int _dataSourceId;

        public const string SHOW_DETAILS_AS_JSON = "SHOW_DETAILS_AS_JSON";
        public const string RENDER_INTERPOLATED_VALUES = "RENDER_INTERPOLATED_VALUES";

        public PiAssetValuesExplorePointViewModel(DataSourceServiceFactory clientFactory, IMessageHub messageHub) : base(messageHub)
        {
            _clientFactory = clientFactory;
            AssetValues = new ObservableCollection<PiAssetValueViewModel>();
            ViewDetailsAsJsonCommand = new DelegateCommand<PiAssetValueViewModel>(OnViewDetailsAsJson);
            RenderInterpolatedValuesCommand = new DelegateCommand<PiAssetValueViewModel>(OnRenderInterpolatedValues);
        }

        private string _messge;
        public string Message
        {
            get { return _messge; }
            set { SetField(ref _messge, value); OnPropertyChanged(nameof(HasMessage)); }
        }

        public bool HasMessage => !string.IsNullOrWhiteSpace(Message);

        public ObservableCollection<PiAssetValueViewModel> AssetValues { get; }

        public ICommand ViewDetailsAsJsonCommand { get; }

        public ICommand RenderInterpolatedValuesCommand { get; }

        public async Task LoadAsync(IPiDetailLoadingInfo loadingInfo) //Assume Id is a link, assume tag is a "type"
        {
            _dataSourceId = loadingInfo.DataSourceId;
            if (loadingInfo.TypeTag == ServerDatabaseAssetWrapper.ASSET_TYPE)
            {
                PiHttpClient client = _clientFactory.GetDataSourceServiceById<PiHttpClient>(loadingInfo.DataSourceId);
                Asset asset = await client.GetByDirectLink<Asset>(loadingInfo.Id);
                await client.LoadAssetValueList(asset);

                Header = asset.Name.First(25) + " (Values)";
                HeaderDetail = $"Value for asset {asset.Name}";
                CurrentLoadingInfo = loadingInfo;

                if (asset.ChildValues.Count() == 0) Message = "No values found for this asset";

                foreach (AssetValue item in asset.ChildValues.OrderBy(x => x.Name))
                {
                    PiAssetValueViewModel vm = new PiAssetValueViewModel(item);
                    AssetValues.Add(vm);
                }
            }
            else
            {
                Header = "Error";
                HeaderDetail = $"Did not know how to render value for a {loadingInfo.TypeTag}";
                throw new ArgumentException(nameof(PiAssetValuesExplorePointViewModel) + " can only render values of an asset");
            }
        }

        private void OnViewDetailsAsJson(PiAssetValueViewModel obj)
        {
            PublishEvent(obj, SHOW_DETAILS_AS_JSON);
        }

        private void OnRenderInterpolatedValues(PiAssetValueViewModel obj)
        {
            PublishEvent(obj, RENDER_INTERPOLATED_VALUES);
        }

        private void PublishEvent(PiAssetValueViewModel obj, string verb)
        {
            AssetValue model = obj.GetBackingModel();
            MessageHub.Publish(new OpenDataSourceViewModelEvent
            {
                Sender = this,
                SenderTypeName = nameof(PiAssetValuesExplorePointViewModel),
                DataSourceId = _dataSourceId,
                Id = model.Links.Source,
                Name = model.Name,
                Verb = verb,
                TypeTag = nameof(AssetValue)
            });
        }
    }
}