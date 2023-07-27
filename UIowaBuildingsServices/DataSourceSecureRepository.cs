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
            return await _dataSourceLocal.GetAllDataSourcesAsync();
        }

        public async Task<DataSourceBase> SaveDataSource(DataSourceBase dataSource)
        {
            dataSource = await _dataSourceClient.SaveDataSource(dataSource);
            dataSource = await _dataSourceLocal.SaveDataSource(dataSource);

            return dataSource;

        }
    }
}

