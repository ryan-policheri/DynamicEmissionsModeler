using EIA.Services.Clients;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorDataAccess.DataSourceWrappers;
using EmissionsMonitorModel.DataSources;
using EmissionsMonitorModel.TimeSeries;
using PiModel;
using PiServices;

namespace EmissionsMonitorServices.DataSourceWrappers
{
    public class DataSourceServiceFactory
    {
        private readonly Func<EiaClient> _eiaClientBuilder;
        private readonly Func<PiHttpClient> _piClientBuilder;
        private readonly IDataSourceRepository _repo;
        private readonly bool _wrap;
        private ICollection<DataSourceInfoAndService> _sources = new List<DataSourceInfoAndService>();

        public DataSourceServiceFactory(Func<EiaClient> eiaClientBuilder, Func<PiHttpClient> piClientBuilder, IDataSourceRepository dataSourceRepo, bool wrap = false)
        {
            _eiaClientBuilder = eiaClientBuilder;
            _piClientBuilder = piClientBuilder;
            _repo = dataSourceRepo;
            _wrap = wrap;
            _sources = new List<DataSourceInfoAndService>();
        }

        public async Task LoadAllDataSourceServices()
        {
            _sources.Clear();
            IEnumerable<DataSourceBase> allDataSources = await _repo.GetAllDataSourcesAsync();
            foreach (DataSourceBase dataSource in allDataSources) UpdateDataSourceService(dataSource);
        }

        public void UpdateDataSourceService(DataSourceBase dataSource)
        {
            if (dataSource == null) throw new ArgumentNullException();

            var existingSource = _sources.FirstOrDefault(x => x.DataSourceInfo.SourceId == dataSource.SourceId);

            if (existingSource == null)
            {
                object dataSourceService = BuildDataSourceService(dataSource);
                _sources.Add(new DataSourceInfoAndService { DataSourceInfo = dataSource, DataSourceService = dataSourceService});
            }
            else
            {
                object dataSourceService = BuildDataSourceService(dataSource);
                _sources.Remove(existingSource);
                _sources.Add(new DataSourceInfoAndService { DataSourceInfo = dataSource, DataSourceService = dataSourceService });
            }
        }

        public DataSourceBase GetDataSourceInfo(int dataSourceId)
        {
            return _sources.Where(x => x.DataSourceInfo.SourceId == dataSourceId).Select(x => new DataSourceBase
            {
                SourceId = x.DataSourceInfo.SourceId,
                SourceName = x.DataSourceInfo.SourceName,
                SourceType = x.DataSourceInfo.SourceType
            }).First();
        }

        public T GetDataSourceServiceById<T>(int dataSourceId) where T : new()
        {
            var existingSource = _sources.First(x => x.DataSourceInfo.SourceId == dataSourceId);
            return (T)existingSource.DataSourceService;
        }

        public ITimeSeriesDataSource GetTimeSeriesDataSourceById(int dataSourceId)
        {
            if (!_wrap) throw new InvalidOperationException();
            var existingSource = _sources.First(x => x.DataSourceInfo.SourceId == dataSourceId);
            return (ITimeSeriesDataSource)existingSource.DataSourceService;
        }

        public object BuildDataSourceService(DataSourceBase dataSourceInfo)
        {
            switch (dataSourceInfo.SourceType)
            {
                case DataSourceType.Eia:
                    IEiaConnectionInfo eiaConnInfo = (EiaDataSource)dataSourceInfo;
                    EiaClient eiaClient = _eiaClientBuilder();
                    eiaClient.Initialize(eiaConnInfo);
                    if (_wrap) return new EiaDataSourceRepoWrapper(eiaClient);
                    else return eiaClient;
                case DataSourceType.Pi:
                    IPiConnectionInfo piConnInfo = (PiDataSource)dataSourceInfo;
                    PiHttpClient piClient = _piClientBuilder();
                    piClient.Initialize(piConnInfo);
                    if (_wrap) return new PiDataSourceRepoWrapper(piClient);
                    else return piClient;
                default:
                    throw new NotImplementedException($"Building service for info type {dataSourceInfo.SourceType} not implemented");
            }
        }

        private class DataSourceInfoAndService
        {
            public DataSourceBase DataSourceInfo { get; set; }
            public object DataSourceService { get; set; }
        }
    }
}
