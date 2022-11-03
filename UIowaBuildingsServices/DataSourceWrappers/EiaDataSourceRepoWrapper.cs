using EmissionsMonitorModel.TimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCommon.Helpers;
using EIA.Services.Clients;
using EmissionsMonitorModel.DataSources;

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

        public Task<Series> GetTimeSeriesAsync(DataSourceSeriesUri uri, TimeSeriesRenderSettings renderSettings)
        {
            throw new NotImplementedException();
        }
    }
}
