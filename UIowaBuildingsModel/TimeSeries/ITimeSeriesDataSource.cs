using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmissionsMonitorModel.TimeSeries
{
    public interface ITimeSeriesDataSource
    {
        public Task<Series> GetTimeSeriesAsync(string uri, DateTimeOffset startTime, DateTimeOffset endTime);
    }
}
