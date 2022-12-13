using EmissionsMonitorModel.DataSources;

namespace EmissionsMonitorDataAccess.Abstractions
{
    public interface IDataSourceRepository
    {
        public Task<IEnumerable<DataSourceBase>> GetAllDataSourcesAsync();

        public Task<DataSourceBase> SaveDataSource(DataSourceBase dataSource);
    }
}
