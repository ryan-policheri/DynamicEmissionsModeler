using System;
using System.Threading.Tasks;
using DotNetCommon.EventAggregation;
using DotNetCommon.SystemHelpers;
using EmissionsMonitorModel.TimeSeries;
using EmissionsMonitorServices.DataSourceWrappers;
using PiModel;
using PiServices;
using UIowaBuildingsServices;
using UnifiedDataExplorer.ModelWrappers;
using UnifiedDataExplorer.Services.DataPersistence;

namespace UnifiedDataExplorer.ViewModel.DataExploring.ExplorePoints
{
    public class PiInterpolatedDataExplorePointViewModel : TimeSeriesExplorePointViewModel
    {
        private readonly DataSourceServiceFactory _clientFactory;
        private PiHttpClient _client;
        private IHaveTimeSeriesData _dataHost;

        public PiInterpolatedDataExplorePointViewModel(DataSourceServiceFactory clientFactory, DataFileProvider dataFileProvider, ExcelExportService exporter, IMessageHub messageHub) : base(dataFileProvider, exporter, messageHub)
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
            TimeSeriesRenderSettings settings = new TimeSeriesRenderSettings
            {
                StartDateTime = new DateTimeOffset(this.StartDateTime.ToUniversalTime()),
                EndDateTime = new DateTimeOffset(this.EndDateTime.ToUniversalTime())
            };
            await _client.LoadInterpolatedValues(_dataHost, settings);
            if(_dataHost is AssetValue) DataSet = (_dataHost as AssetValue).RenderDataPointsAsTable();
            if(_dataHost is PiPoint) DataSet = (_dataHost as PiPoint).RenderDataPointsAsTable();
        }
    }
}
