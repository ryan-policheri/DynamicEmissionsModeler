using EmissionsMonitorModel.TimeSeries;
using DotNetCommon.Helpers;
using EIA.Services.Clients;
using EmissionsMonitorModel.DataSources;
using System;

namespace EmissionsMonitorDataAccess.DataSourceWrappers
{
    public class EiaDataSourceRepoWrapper : ITimeSeriesDataSource
    {
        private EiaClient _client;
        private IEiaConnectionInfo _info;

        public EiaDataSourceRepoWrapper(EiaClient client)
        {
            _client = client;
        }

        public void Initialize(IEiaConnectionInfo connInfo)
        {
            _info = connInfo;
            _client.Initialize(_info);
        }

        public async Task<Series> GetTimeSeriesAsync(DataSourceSeriesUri uri, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            EIA.Domain.Model.Series eiaSeries = await _client.GetHourlySeriesByIdAsync(uri.Uri, startTime, endTime, TimeZones.GetUtcOffset());
            Series series = new Series();
            series.SeriesUri = uri;
            //TODO: Some error checking on the data points
            series.DataPoints = eiaSeries.DataPoints.Select(x => new DataPoint
            {
                Series = series,
                Timestamp = x.Timestamp,
                Value = x.Value.Value
            }).ToList();
            return series;
        }

        public async Task<Series> GetTimeSeriesAsync(DataSourceSeriesUri uri, QueryRenderSettings renderSettings)
        {
            EIA.Domain.Model.Series eiaSeries = await _client.GetSeriesByIdAsync(uri.Uri, renderSettings);
            Series series = new Series();
            series.SeriesUri = uri;
            //TODO: Some error checking on the data points
            series.DataPoints = eiaSeries.DataPoints.Select(x => new DataPoint
            {
                Series = series,
                Timestamp = x.Timestamp,
                Value = x.Value.HasValue ? x.Value.Value : 0
            }).ToList();
            return series;
        }

        public async Task<Series> GetTimeSeriesAsync(DataSourceSeriesUriQueryRender query)
        {
            EIA.Domain.Model.Series eiaSeries = await _client.GetSeriesByIdAsync(query.Uri, query);
            Series series = new Series();
            series.SeriesUri = query;
            //TODO: Some error checking on the data points
            series.DataPoints = eiaSeries.DataPoints.Select(x => new DataPoint
            {
                Series = series,
                Timestamp = x.Timestamp,
                Value = x.Value.HasValue ? x.Value.Value : 0
            }).ToList();
            return series;
        }

        public EiaClient GetUnderlyingClient() => _client;
    }
}
