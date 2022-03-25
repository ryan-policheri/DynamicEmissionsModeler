using System;
using System.Data;
using System.Threading.Tasks;
using DotNetCommon.EventAggregation;
using PiModel;
using PiServices;
using UnifiedDataExplorer.ModelWrappers;

namespace UnifiedDataExplorer.ViewModel
{
    public class PiInterpolatedDataViewModel : ExplorePointViewModel
    {
        private readonly PiHttpClient _client;

        public PiInterpolatedDataViewModel(PiHttpClient client, IMessageHub messageHub) : base(messageHub)
        {
            _client = client;
        }

        private string _messge;
        public string Message
        {
            get { return _messge; }
            set { SetField<string>(ref _messge, value); OnPropertyChanged(nameof(HasMessage)); }
        }

        public bool HasMessage => !String.IsNullOrWhiteSpace(this.Message);

        private string _assetName;
        public string AssetName 
        { 
            get { return _assetName; }
            private set { SetField<string>(ref _assetName, value); }
        }

        private string _valueName;
        public string ValueName 
        {
            get { return _valueName; }
            private set { SetField<string> (ref _valueName, value); }
        }

        private DataTable _dataSet;
        public DataTable DataSet
        {
            get { return _dataSet; }
            private set { SetField<DataTable>(ref _dataSet, value); }
        }

        public async Task LoadAsync(IPiDetailLoadingInfo loadingInfo)
        {
            if (loadingInfo.TypeTag == nameof(AssetValue))
            {
                CurrentLoadingInfo = loadingInfo;
                AssetValue value = await this._client.GetByDirectLink<AssetValue>(loadingInfo.Id);
                Asset asset = await this._client.GetByDirectLink<Asset>(value.Links.Element);
                Header = asset.Name + " - " + value.Name;
                HeaderDetail = $"Interpolated {value.Name} data for {asset.Name}";
                AssetName = asset.Name;
                ValueName = value.Name;
                await this._client.LoadInterpolatedValues(value, 30, false);
                DataSet = value.RenderDataPointsAsTable();
            }
            else if(loadingInfo.TypeTag == PiPoint.PI_POINT_TYPE)
            {
                CurrentLoadingInfo = loadingInfo;
                PiPoint piPoint = await this._client.GetByDirectLink<PiPoint>(loadingInfo.Id);
                await _client.LoadInterpolatedValues(piPoint, 30, false);
                Header = piPoint.Name;
                HeaderDetail = $"Interpolated {piPoint.Name} data for {piPoint.Name}";
                AssetName = piPoint.Name;
                ValueName = piPoint.Descriptor;
                await this._client.LoadInterpolatedValues(piPoint, 30, false);
                DataSet = piPoint.RenderDataPointsAsTable();
            }
            else
            {
                Header = "Error";
                HeaderDetail = $"Did not know how to render value for a {loadingInfo.TypeTag}";
                throw new ArgumentException(nameof(PiInterpolatedDataViewModel) + " can only render values of an asset");
            }
        }
    }
}
