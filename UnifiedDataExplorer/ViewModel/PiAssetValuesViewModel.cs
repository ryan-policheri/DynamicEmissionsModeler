using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DotNetCommon.EventAggregation;
using DotNetCommon.Extensions;
using PiModel;
using PiServices;
using UnifiedDataExplorer.ModelWrappers;

namespace UnifiedDataExplorer.ViewModel
{
    public class PiAssetValuesViewModel : ExplorePointViewModel
    {
        private readonly PiHttpClient _client;

        public PiAssetValuesViewModel(PiHttpClient client, IMessageHub messageHub) : base(messageHub)
        {
            _client = client;
            AssetValues = new ObservableCollection<PiAssetValueViewModel>();
        }

        public ObservableCollection<PiAssetValueViewModel> AssetValues { get; }

        public async Task LoadAsync(IPiDetailLoadingInfo loadingInfo) //Assume Id is a link, assume tag is a "type"
        {
            if (loadingInfo.Tag == ServerDatabaseAssetWrapper.ASSET_TYPE)
            {
                Asset asset = await _client.GetBySelfLink<Asset>(loadingInfo.Id);
                await _client.LoadAssetValues(asset);

                Header = asset.Name.First(25) + " (Values)";
                HeaderDetail = $"Value for asset {asset.Name}";

                foreach (AssetValue item in asset.ChildValues)
                {
                    PiAssetValueViewModel vm = new PiAssetValueViewModel(item);
                    AssetValues.Add(vm);
                }
            }
            else
            {
                Header = "Error";
                HeaderDetail = $"Did not know how to render value for a {loadingInfo.Tag}";
                throw new ArgumentException(nameof(PiAssetValuesViewModel) + " can only render values of an asset");
            }
        }
    }
}