using DotNetCommon.Extensions;
using EIA.Services.Clients;
using EmissionsMonitorModel.DataSources;
using PiModel;
using PiServices;

namespace EmissionsMonitorServices.DataSourceWrappers
{
    public class DataSourceServiceFactory
    {
        private readonly Func<EiaClient> _eiaClientBuilder;
        private readonly Func<PiHttpClient> _piClientBuilder;
        private ICollection<DataSourceInfoAndService> _sources = new List<DataSourceInfoAndService>();

        public DataSourceServiceFactory(Func<EiaClient> eiaClientBuilder, Func<PiHttpClient> piClientBuilder)
        {
            _eiaClientBuilder = eiaClientBuilder;
            _piClientBuilder = piClientBuilder;
            _sources = new List<DataSourceInfoAndService>();
        }

        public T ConstructDataSourceService<T>(DataSourceBase dataSource) where T : new()
        {
            if (dataSource == null) throw new ArgumentNullException();

            var existingSource = _sources.First(x => x.DataSourceInfo.SourceId == dataSource.SourceId);

            if (existingSource == null)
            {
                object dataSourceService = BuildDataSourceService<T>(dataSource);
                _sources.Add(new DataSourceInfoAndService { DataSourceInfo = dataSource, DataSourceService = dataSourceService});
            }
            else
            {
                object dataSourceService = BuildDataSourceService<T>(dataSource);
                _sources.Remove(existingSource);
                _sources.Add(new DataSourceInfoAndService { DataSourceInfo = dataSource, DataSourceService = dataSourceService });
            }

            return GetDataSourceServiceById<T>(dataSource.SourceId);
        }

        public T GetDataSourceServiceById<T>(int dataSourceId) where T : new()
        {
            var existingSource = _sources.First(x => x.DataSourceInfo.SourceId == dataSourceId);
            return (T)existingSource.DataSourceService;
        }

        public T BuildDataSourceService<T>(DataSourceBase dataSourceInfo) where T : new()
        {
            switch (dataSourceInfo.SourceType)
            {
                case DataSourceType.Eia:
                    if (typeof(T) != typeof(EiaClient)) throw new InvalidOperationException($"Unxpected type {nameof(T)} for data source type{DataSourceType.Eia.ToDescription()}");
                    IEiaConnectionInfo eiaConnInfo = (EiaDataSource)dataSourceInfo;
                    EiaClient eiaClient = _eiaClientBuilder();
                    eiaClient.Initialize(eiaConnInfo);
                    return (T)Convert.ChangeType(eiaClient, typeof(T));
                case DataSourceType.Pi:
                    if (typeof(T) != typeof(PiHttpClient)) throw new InvalidOperationException($"Unxpected type {nameof(T)} for data source type{DataSourceType.Pi.ToDescription()}");
                    IPiConnectionInfo piConnInfo = (PiDataSource)dataSourceInfo;
                    PiHttpClient piClient = _piClientBuilder();
                    piClient.Initialize(piConnInfo);
                    return (T)Convert.ChangeType(piClient, typeof(T));
                default:
                    throw new NotImplementedException($"Building service {nameof(T)} and for info type {dataSourceInfo.SourceType} not implemented");
            }
        }

        private class DataSourceInfoAndService
        {
            public DataSourceBase DataSourceInfo { get; set; }
            public object DataSourceService { get; set; }
        }
    }
}
