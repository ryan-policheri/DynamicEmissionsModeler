using DotNetCommon.Security;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.DataSources;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EmissionsMonitorDataAccess.FileSystem
{
    public class DataSourceLocal : IDataSourceRepository
    {

        private ICredentialProvider _credentialProvider;
        private string _appDataDirectory;
        private string _fileName = "DataSources.json";
        private string _fileDirectory;

        public DataSourceLocal(ICredentialProvider credentialProvider)
        {
            _credentialProvider = credentialProvider;
        }

        public void Initialize(IEmissionsMonitorLocalConfig config)
        {
            _appDataDirectory = config.AppDataDirectory;
            _fileDirectory = Path.Combine(_appDataDirectory, _fileName);
        }

        public async Task<IEnumerable<DataSourceBase>> GetAllDataSourcesAsync()
        {
            DataSourcesJson dataSourcesJson;

            if (File.Exists(_fileDirectory))
            {
                string inFile = File.ReadAllText(_fileDirectory);
                dataSourcesJson = JsonSerializer.Deserialize<DataSourcesJson>(inFile);
                foreach (DataSourceBase dataSourceBase in dataSourcesJson.DataSources)
                {
                    dataSourceBase.SourceDetailsJson = _credentialProvider.DecryptValue(dataSourceBase.SourceDetailsJson);
                }
            }
            else
            {
                dataSourcesJson = new DataSourcesJson();
            }

            return dataSourcesJson.DataSources.Select(x => x.FromSourceDetails()).ToList();
        }

        public async Task<DataSourceBase> SaveDataSource(DataSourceBase dataSource)
        {
            // get existing data sources
            DataSourcesJson dataSourcesJson;
            if (File.Exists(_fileDirectory))
            {
                string inFile = File.ReadAllText(_fileDirectory);
                dataSourcesJson = JsonSerializer.Deserialize<DataSourcesJson>(inFile);
            }
            else
            {
                dataSourcesJson = new DataSourcesJson();
            }

            // remove dataSource if it already exists
            dataSourcesJson.DataSources.RemoveAll(x => x.SourceId == dataSource.SourceId);

            // encrypt then write back to file system
            dataSourcesJson.DataSources.Add(EncryptDataSource(dataSource));
            string serialized = JsonSerializer.Serialize(dataSourcesJson);
            File.WriteAllText(_fileDirectory, serialized);

            return dataSource;
        }

        public DataSourceBase EncryptDataSource(DataSourceBase dataSource)
        {
            dataSource.SourceDetailsJson = _credentialProvider.EncryptValue(dataSource.SourceDetailsJson);
            return dataSource;
        }

    }
}