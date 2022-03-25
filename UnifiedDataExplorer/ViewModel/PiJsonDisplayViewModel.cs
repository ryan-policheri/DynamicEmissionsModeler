using System;
using System.Threading.Tasks;
using DotNetCommon.EventAggregation;
using DotNetCommon.Extensions;
using PiModel;
using PiServices;
using UnifiedDataExplorer.ModelWrappers;

namespace UnifiedDataExplorer.ViewModel
{
    public class PiJsonDisplayViewModel : ExplorePointViewModel
    {
        private readonly PiHttpClient _client;

        public PiJsonDisplayViewModel(PiHttpClient client, IMessageHub messageHub) : base(messageHub)
        {
            _client = client;
        }

        private string _json;
        public string Json 
        { 
            get { return _json; }
            private set { SetField(ref _json, value); }
        }

        public async Task LoadAsync(IPiDetailLoadingInfo loadingInfo) //Assume Id is a link, assume tag is a "type"
        {
            switch(loadingInfo.TypeTag)
            {
                case ServerDatabaseAssetWrapper.ASSET_SERVER_TYPE:
                case ServerDatabaseAssetWrapper.DATABASE_TYPE:
                case ServerDatabaseAssetWrapper.ASSET_TYPE:
                    ItemBase itemBase;
                    if (loadingInfo.TypeTag == ServerDatabaseAssetWrapper.ASSET_SERVER_TYPE) itemBase = await _client.GetByDirectLink<AssetServer>(loadingInfo.Id);
                    else if (loadingInfo.TypeTag == ServerDatabaseAssetWrapper.DATABASE_TYPE) itemBase = await _client.GetByDirectLink<Database>(loadingInfo.Id);
                    else if (loadingInfo.TypeTag == ServerDatabaseAssetWrapper.ASSET_TYPE) itemBase = await _client.GetByDirectLink<Asset>(loadingInfo.Id);
                    else if (loadingInfo.TypeTag == nameof(AssetValue)) itemBase = await _client.GetByDirectLink<AssetValue>(loadingInfo.Id);
                    else throw new NotImplementedException("Don't know how to get pi model from the loading info");
                    Json = itemBase.ToBeautifulJson();
                    Header = itemBase.Name.First(25) + " (Json)";
                    HeaderDetail = itemBase.Name + " (Json)";
                    break;
                case PiPoint.PI_POINT_TYPE:
                    PiPoint piPoint = await _client.GetByDirectLink<PiPoint>(loadingInfo.Id);
                    Json = piPoint.ToBeautifulJson();
                    Header = piPoint.Name.First(25) + " (Json)";
                    HeaderDetail = piPoint.Descriptor;
                    break;
            }

            CurrentLoadingInfo = loadingInfo;
        }
    }
}