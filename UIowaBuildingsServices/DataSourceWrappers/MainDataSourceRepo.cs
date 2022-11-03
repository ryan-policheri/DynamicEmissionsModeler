using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmissionsMonitorModel.DataSources;
using EmissionsMonitorModel.TimeSeries;

namespace EmissionsMonitorServices.DataSourceWrappers
{
    public class MainDataSourceRepo : ITimeSeriesDataSource
    {
        private readonly DataSourceServiceFactory _dataSourceFactory;

        public MainDataSourceRepo(DataSourceServiceFactory dataSourceFactory)
        {
            _dataSourceFactory = dataSourceFactory;
        }

        public async Task<Series> GetTimeSeriesAsync(DataSourceSeriesUri uri, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            ITimeSeriesDataSource dataSource = _dataSourceFactory.GetTimeSeriesDataSourceById(uri.DataSourceId);
            Series data = await dataSource.GetTimeSeriesAsync(uri, startTime, endTime);
            return data;
        }

        public async Task<Series> GetTimeSeriesAsync(DataSourceSeriesUri uri, TimeSeriesRenderSettings renderSettings)
        {
            ITimeSeriesDataSource dataSource = _dataSourceFactory.GetTimeSeriesDataSourceById(uri.DataSourceId);
            Series data = await dataSource.GetTimeSeriesAsync(uri, renderSettings);
            return data;
        }
    }
}
