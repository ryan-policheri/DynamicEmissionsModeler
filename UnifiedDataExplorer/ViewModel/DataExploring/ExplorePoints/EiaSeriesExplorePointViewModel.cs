using System;
using System.Threading.Tasks;
using DotNetCommon.EventAggregation;
using EIA.Services.Clients;
using EmissionsMonitorModel.DataSources;
using UnifiedDataExplorer.ModelWrappers;
using UnifiedDataExplorer.Services.DataPersistence;
using UIowaBuildingsServices;
using EmissionsMonitorServices.DataSourceWrappers;
using EmissionsMonitorModel.TimeSeries;

namespace UnifiedDataExplorer.ViewModel.DataExploring.ExplorePoints
{
    public class EiaSeriesExplorePointViewModel : TimeSeriesExplorePointViewModel
    {
        private readonly DataSourceServiceFactory _clientFactory;
        private EiaClient _client;
        private EIA.Domain.Model.Series _series;

        public EiaSeriesExplorePointViewModel(DataSourceServiceFactory clientFactory, DataFileProvider dataFileProvider, ExcelExportService exporter, IMessageHub messageHub) : base(dataFileProvider, exporter, messageHub)
        {
            _clientFactory = clientFactory; 
        }

        public async Task LoadAsync(IEiaDetailLoadingInfo loadingInfo)
        {
            CurrentLoadingInfo = loadingInfo;
            _client = _clientFactory.GetDataSourceServiceById<EiaClient>(loadingInfo.DataSourceId);
            _series = await _client.GetSeriesByIdAsync(loadingInfo.Id, 2);
            Header = _series.Id;
            HeaderDetail = _series.Name;
            SeriesName = _series.Id;
            SeriesDescription = _series.Description;
            UnitsSummary = _series.Units;
            await RenderDataSet();
        }

        protected override async Task RenderDataSet()
        {
            TimeSeriesRenderSettings settings = new TimeSeriesRenderSettings
            {
                StartDateTime = new DateTimeOffset(this.StartDateTime.ToUniversalTime()),
                EndDateTime = new DateTimeOffset(this.EndDateTime.ToUniversalTime())
            };
            _series = await _client.GetSeriesByIdAsync(_series.Id, 30);
            DataSet = _series.RenderDataPointsAsTable();
        }

        public override DataSourceSeriesUri BuildSeriesUri()
        {
            var loadingInfo = (this.CurrentLoadingInfo as IEiaDetailLoadingInfo);
            var uri = new DataSourceSeriesUri
            {
                DataSourceId = loadingInfo.DataSourceId,
                Prefix = null,
                SeriesName = SeriesName,
                Uri = loadingInfo.Id,
                SeriesUnitsSummary = UnitsSummary,
            };
            return uri;
        }
    }
}