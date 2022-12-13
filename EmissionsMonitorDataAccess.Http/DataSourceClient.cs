using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.DataSources;
using EmissiosMonitorDataAccess.Http;

namespace EmissionsMonitorDataAccess.Http
{
    public class DataSourceClient : EmissionsMonitorClient, IDataSourceRepository
    {
        public async Task<IEnumerable<DataSourceBase>> GetAllDataSourcesAsync()
        {
            ICollection<DataSourceBase> dataSources = (await this.GetAllAsync<DataSourceBase>("DataSource")).ToList();
            return dataSources.Select(x => x.FromSourceDetails()).ToList();
        }

        public async Task<DataSourceBase> SaveDataSource(DataSourceBase dataSource) => await PostDataSource(dataSource);

        public async Task<DataSourceBase> PostDataSource(DataSourceBase dataSource)
        {
            dataSource.SourceDetailsJson = dataSource.ToSourceDetails();
            return await this.PostAsync<DataSourceBase>("DataSource", dataSource);
        }
    }
}