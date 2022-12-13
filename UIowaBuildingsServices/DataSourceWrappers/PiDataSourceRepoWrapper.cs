using EmissionsMonitorModel.TimeSeries;
using EmissionsMonitorModel.DataSources;
using PiServices;
using PiModel;
using EmissionsMonitorModel.ProcessModeling;

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
            if (uri.Prefix == PiPoint.PI_POINT_TYPE)
            {
                PiPoint inputPiPoint = await _client.GetByDirectLink<PiPoint>(uri.Uri);
                await _client.LoadInterpolatedValues(inputPiPoint, startTime, endTime);
                Series series = new Series();
                series.SeriesUri = uri;
                //TODO: Some error checking on the data points
                series.DataPoints = inputPiPoint.InterpolatedDataPoints.Select(x => new DataPoint
                {
                    Series = series,
                    Timestamp = x.Timestamp,
                    Value = x.Value
                }).ToList();
                return series;
            }
            else if (uri.Prefix == PiDataSource.PI_AF_PREFIX)
            {
                throw new NotImplementedException("Implement this");
            }
            else throw new ArgumentException($"Pi Prefix \"{uri.Prefix}\" unknown");
        }

        public async Task<Series> GetTimeSeriesAsync(DataSourceSeriesUri uri, QueryRenderSettings renderSettings)
        {
            if (uri.Prefix == PiPoint.PI_POINT_TYPE)
            {
                PiPoint inputPiPoint = await _client.GetByDirectLink<PiPoint>(uri.Uri);
                await _client.LoadSummaryValues(inputPiPoint, renderSettings);
                Series series = new Series();
                series.SeriesUri = uri;
                //TODO: Some error checking on the data points
                series.DataPoints = inputPiPoint.SummaryDataPoints.Select(x => new DataPoint
                {
                    Series = series,
                    Timestamp = x.DataPoint.Timestamp,
                    Value = x.HasErrors ? 0 : double.Parse(x.DataPoint.Value.ToString())
                }).ToList();
                return series;
            }
            else if (uri.Prefix == PiDataSource.PI_AF_PREFIX)
            {
                throw new NotImplementedException("Implement this");
            }
            else throw new ArgumentException($"Pi Prefix \"{uri.Prefix}\" unknown");
        }

        public async Task<Series> GetTimeSeriesAsync(DataSourceSeriesUriQueryRender query)
        {
            if (query.Prefix == PiPoint.PI_POINT_TYPE)
            {
                PiPoint inputPiPoint = await _client.GetByDirectLink<PiPoint>(query.Uri);
                await _client.LoadSummaryValues(inputPiPoint, query);
                Series series = new Series();
                series.SeriesUri = query;
                //TODO: Some error checking on the data points
                series.DataPoints = inputPiPoint.SummaryDataPoints.Select(x => 
                {
                    double value = x.HasErrors ? 0 : double.Parse(x.DataPoint.Value.ToString());
                    //if (x?.DataPoint?.Value == null)
                    //{
                    //    if (query.UseZeroForNulls) value = 0;
                    //    else throw new NullReferenceException("No value found for during enumeration of " + query.SeriesName);
                    //}

                    if(value < 0 && query.UseZeroForNegatives) value = 0;
                    return new DataPoint
                    {
                        Series = series,
                        Timestamp = x.DataPoint.Timestamp,
                        Value = value
                    };
                }).ToList();
                return series;
            }
            else if (query.Prefix == PiDataSource.PI_AF_PREFIX)
            {
                throw new NotImplementedException("Implement this");
            }
            else throw new ArgumentException($"Pi Prefix \"{query.Prefix}\" unknown");
        }

        public PiHttpClient GetUnderlyingClient() => _client;
    }
}
