using DotNetCommon.WebApiClient;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorDataAccess.Http;
using EmissionsMonitorModel.DataSources;

namespace EmissiosMonitorDataAccess.Http
{
    public class EmissionsMonitorClient : WebApiClientBase, IDataSourceRepository
    {
        public EmissionsMonitorClient()
        {
            this.Client = new HttpClient();
            this.SerializerOptions = JsonSerializerDefaults.CamelCaseOptions;
        }

        public void Initialize(IEmissionsMonitorClientConfig config)
        {
            this.Client.BaseAddress = new Uri(config.EmissionsMonitorApiBaseUrl);
        }

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