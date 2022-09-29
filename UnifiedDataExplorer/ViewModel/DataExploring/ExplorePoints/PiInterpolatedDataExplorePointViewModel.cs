using System;
using System.Data;
using System.Threading.Tasks;
using DotNetCommon.EventAggregation;
using EmissionsMonitorServices.DataSourceWrappers;
using PiModel;
using PiServices;
using UnifiedDataExplorer.ModelWrappers;

namespace UnifiedDataExplorer.ViewModel.DataExploring.ExplorePoints
{
    public class PiInterpolatedDataExplorePointViewModel : ExplorePointViewModel
    {
        private readonly DataSourceServiceFactory _clientFactory;

        public PiInterpolatedDataExplorePointViewModel(DataSourceServiceFactory clientFactory, IMessageHub messageHub) : base(messageHub)
        {
            _clientFactory = clientFactory;
        }

        private string _messge;
        public string Message
        {
            get { return _messge; }
            set { SetField(ref _messge, value); OnPropertyChanged(nameof(HasMessage)); }
        }

        public bool HasMessage => !string.IsNullOrWhiteSpace(Message);

        private string _assetName;
        public string AssetName
        {
            get { return _assetName; }
            private set { SetField(ref _assetName, value); }
        }

        private string _valueName;
        public string ValueName
        {
            get { return _valueName; }
            private set { SetField(ref _valueName, value); }
        }

        private DataTable _dataSet;
        public DataTable DataSet
        {
            get { return _dataSet; }
            private set { SetField(ref _dataSet, value); }
        }

        public async Task LoadAsync(IPiDetailLoadingInfo loadingInfo)
        {
            PiHttpClient client = _clientFactory.GetDataSourceServiceById<PiHttpClient>(loadingInfo.DataSourceId);
            if (loadingInfo.TypeTag == nameof(AssetValue))
            {
                CurrentLoadingInfo = loadingInfo;
                AssetValue value = await client.GetByDirectLink<AssetValue>(loadingInfo.Id);
                Asset asset = await client.GetByDirectLink<Asset>(value.Links.Element);
                Header = asset.Name + " - " + value.Name;
                HeaderDetail = $"Interpolated {value.Name} data for {asset.Name}";
                AssetName = asset.Name;
                ValueName = value.Name;
                await client.LoadInterpolatedValues(value, 30);
                DataSet = value.RenderDataPointsAsTable();
            }
            else if (loadingInfo.TypeTag == PiPoint.PI_POINT_TYPE)
            {
                CurrentLoadingInfo = loadingInfo;
                PiPoint piPoint = await client.GetByDirectLink<PiPoint>(loadingInfo.Id);
                await client.LoadInterpolatedValues(piPoint, 30);
                Header = piPoint.Name;
                HeaderDetail = $"Interpolated {piPoint.Name} data for {piPoint.Name}";
                AssetName = piPoint.Name;
                ValueName = piPoint.Descriptor;
                await client.LoadInterpolatedValues(piPoint, 30);
                DataSet = piPoint.RenderDataPointsAsTable();
            }
            else
            {
                Header = "Error";
                HeaderDetail = $"Did not know how to render value for a {loadingInfo.TypeTag}";
                throw new ArgumentException(nameof(PiInterpolatedDataExplorePointViewModel) + " can only render values of an asset");
            }
        }
    }
}
