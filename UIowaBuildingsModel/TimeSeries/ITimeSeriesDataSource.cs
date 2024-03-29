﻿using EmissionsMonitorModel.DataSources;

namespace EmissionsMonitorModel.TimeSeries
{
    public interface ITimeSeriesDataSource
    {
        public Task<Series> GetTimeSeriesAsync(DataSourceSeriesUri uri, DateTimeOffset startTime, DateTimeOffset endTime);

        public Task<Series> GetTimeSeriesAsync(DataSourceSeriesUri uri, QueryRenderSettings renderSettings);

        public Task<Series> GetTimeSeriesAsync(DataSourceSeriesUriQueryRender query);
    }
}