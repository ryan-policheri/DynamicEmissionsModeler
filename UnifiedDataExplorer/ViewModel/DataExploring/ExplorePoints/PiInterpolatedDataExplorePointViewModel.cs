using System;
using System.Threading.Tasks;
using DotNetCommon.EventAggregation;
using EmissionsMonitorServices.DataSourceWrappers;
using PiModel;
using PiServices;
using UnifiedDataExplorer.ModelWrappers;

namespace UnifiedDataExplorer.ViewModel.DataExploring.ExplorePoints
{
    public class PiInterpolatedDataExplorePointViewModel : TimeSeriesExplorePointViewModel
    {
        private readonly DataSourceServiceFactory _clientFactory;
        private PiHttpClient _client;
        private IHaveTimeSeriesData _dataHost;

        public PiInterpolatedDataExplorePointViewModel(DataSourceServiceFactory clientFactory, IMessageHub messageHub) : base(messageHub)
        {
            _clientFactory = clientFactory;
        }

        public async Task LoadAsync(IPiDetailLoadingInfo loadingInfo)
        {
            PiHttpClient client = _clientFactory.GetDataSourceServiceById<PiHttpClient>(loadingInfo.DataSourceId);
            _client = client;

            if (loadingInfo.TypeTag == nameof(AssetValue))
            {
                CurrentLoadingInfo = loadingInfo;
                AssetValue value = await client.GetByDirectLink<AssetValue>(loadingInfo.Id);
                Asset asset = await client.GetByDirectLink<Asset>(value.Links.Element);
                Header = asset.Name + " - " + value.Name;
                HeaderDetail = $"Interpolated {value.Name} data for {asset.Name}";
                SeriesName = asset.Name + " - " + value.Name;
                UnitsSummary = value.DefaultUnitsName;
                _dataHost = value;
                await RenderDataSet();
            }
            else if (loadingInfo.TypeTag == PiPoint.PI_POINT_TYPE)
            {
                CurrentLoadingInfo = loadingInfo;
                PiPoint piPoint = await client.GetByDirectLink<PiPoint>(loadingInfo.Id);
                await client.LoadInterpolatedValues(piPoint, 30);
                Header = piPoint.Name;
                HeaderDetail = $"Interpolated {piPoint.Name} data for {piPoint.Name}";
                SeriesName = piPoint.Name;
                UnitsSummary = piPoint.EngineeringUnits;
                _dataHost = piPoint;
                await RenderDataSet();
            }
            else
            {
                Header = "Error";
                HeaderDetail = $"Did not know how to render value for a {loadingInfo.TypeTag}";
                throw new ArgumentException(nameof(PiInterpolatedDataExplorePointViewModel) + " can only render values of an asset");
            }
        }

        protected override async Task RenderDataSet()
        {
            await _client.LoadInterpolatedValues(_dataHost, 30);
            if(_dataHost is AssetValue) DataSet = (_dataHost as AssetValue).RenderDataPointsAsTable();
            if(_dataHost is PiPoint) DataSet = (_dataHost as PiPoint).RenderDataPointsAsTable();
        }
    }
}
