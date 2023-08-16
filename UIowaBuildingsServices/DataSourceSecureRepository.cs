using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmissionsMonitorDataAccess.Http;
using EmissionsMonitorDataAccess.FileSystem;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.DataSources;

namespace EmissionsMonitorServices
{
    public class DataSourceSecureRepository : IDataSourceRepository
    {

        private DataSourceClient _dataSourceClient;
        private DataSourceLocal _dataSourceLocal;

        public DataSourceSecureRepository(DataSourceClient dataSourceClient, DataSourceLocal dataSourceLocal)
        {
            _dataSourceClient = dataSourceClient;
            _dataSourceLocal = dataSourceLocal;
        }

        public async Task<IEnumerable<DataSourceBase>> GetAllDataSourcesAsync()
        {
            // Returns all datasources. If datasource exists on the server and locally, then datasource with 
            // credentials are returned. Otherwise (meaning they are stored on the server but not locally), they are returned without 
            // credentials.

            IEnumerable<DataSourceBase> dataSourcesServerResult = await _dataSourceClient.GetAllDataSourcesAsync();
            IEnumerable<DataSourceBase> dataSourcesLocalResult = await _dataSourceLocal.GetAllDataSourcesAsync();

            List<DataSourceBase> dataSourcesServer = dataSourcesServerResult.ToList();
            List<DataSourceBase> dataSourcesLocal = dataSourcesLocalResult.ToList();

            List<DataSourceBase> dataSources = new List<DataSourceBase>();

            foreach (DataSourceBase dataSourceServer in dataSourcesServer)
            {
                if (dataSourcesLocal.Select(x => x.SourceId).Contains(dataSourceServer.SourceId)) {
                    DataSourceBase localDataSource = dataSourcesLocal.Where(x => x.SourceId == dataSourceServer.SourceId).First();
                    dataSources.Add(localDataSource);
                }
                else
                {
                    dataSources.Add(dataSourceServer);
                }
            } 

            return dataSources;
        }

        public async Task<DataSourceBase> SaveDataSource(DataSourceBase dataSource)
        {

            string sourceDetails = dataSource.ToSourceDetails();
            dataSource = await _dataSourceClient.SaveDataSource(dataSource);
            dataSource.SourceDetailsJson = sourceDetails;
            dataSource = await _dataSourceLocal.SaveDataSource(dataSource);

            return dataSource;

        }
    }
}

