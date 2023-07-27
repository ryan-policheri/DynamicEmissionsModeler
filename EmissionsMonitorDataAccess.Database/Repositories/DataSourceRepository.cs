using DotNetCommon.Security;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.DataSources;
using Microsoft.EntityFrameworkCore;

namespace EmissionsMonitorDataAccess.Database.Repositories
{
    public class DataSourceRepository : GenericRepository<DataSourceBase, EmissionsMonitorContext>, IDataSourceRepository
    {
        private readonly ICredentialProvider _credentialProvider;

        public DataSourceRepository(EmissionsMonitorContext context, ICredentialProvider credentialProvider) : base(context)
        {
            _credentialProvider = credentialProvider;
        }

        public async Task<IEnumerable<DataSourceBase>> GetAllDataSourcesAsync()
        {
            IEnumerable<DataSourceBase> dataSources = await Context.Set<DataSourceBase>().ToListAsync();
            return dataSources
                .Select(x => DecryptDataSource(x))
                .Select(x => x.FromSourceDetails());
        }

        public async Task<DataSourceBase> SaveDataSource(DataSourceBase dataSource) => await UpsertDataSource(dataSource);

        public async Task<DataSourceBase> UpsertDataSource(DataSourceBase source)
        {
            DataSourceBase existing = await this.GetByIdAsync(source.SourceId);
            if (existing == null) Add(EncryptDataSource(source));
            else
            {
                Remove(existing);
                Add(EncryptDataSource(source));
            }
            await Context.SaveChangesAsync();
            return DecryptDataSource(source);
        }

        public DataSourceBase EncryptDataSource(DataSourceBase dataSource)
        {
            dataSource.SourceDetailsJson = _credentialProvider.EncryptValue(dataSource.SourceDetailsJson);
            return dataSource;
        }

        public DataSourceBase DecryptDataSource(DataSourceBase dataSource)
        {
            dataSource.SourceDetailsJson = _credentialProvider.DecryptValue(dataSource.SourceDetailsJson);
            return dataSource;
        }
    }
}
