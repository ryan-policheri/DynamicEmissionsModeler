﻿using EmissionsMonitorModel.TimeSeries;
using EmissionsMonitorModel.DataSources;
using PiServices;
using PiModel;

namespace EmissionsMonitorDataAccess.DataSourceWrappers
{
    public class PiDataSourceRepoWrapper : ITimeSeriesDataSource
    {
        private readonly PiHttpClient _client;
        private PiDataSource _info;

        public PiDataSourceRepoWrapper(PiHttpClient client)
        {
            _client = client;
        }

        public void Initialize(PiDataSource sourceInfo)
        {
            _info = sourceInfo;
            _client.Initialize(_info);
        }

        public async Task<Series> GetTimeSeriesAsync(DataSourceSeriesUri uri, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            if (uri.Prefix == PiDataSource.PI_TAG_PREFIX)
            {
                PiPoint inputPiPoint = await _client.SearchPiPoint(uri.Uri);
                await _client.LoadInterpolatedValues(inputPiPoint, startTime, endTime);
                Series series = new Series();
                series.SeriesUri = uri;
                //TODO: Some error checking on the data points
                series.DataPoints = inputPiPoint.InterpolatedDataPoints.Select(x => new DataPoint
                {
                    Series = series,
                    Timestamp = x.Timestamp,
                    Value = x.Value
                });
                return series;
            }
            else if (uri.Prefix == PiDataSource.PI_AF_PREFIX)
            {
                throw new NotImplementedException("Implement this");
            }
            else throw new ArgumentException($"Pi Prefix \"{uri.Prefix}\" unknown");
        }
    }
}