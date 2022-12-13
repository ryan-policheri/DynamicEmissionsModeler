using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.DataSources;
using Microsoft.EntityFrameworkCore;

namespace EmissionsMonitorDataAccess.Database.Repositories
{
    public class DataSourceRepository : GenericRepository<DataSourceBase, EmissionsMonitorContext>, IDataSourceRepository
    {
        public DataSourceRepository(EmissionsMonitorContext context) : base(context)
        {
        }

        public async Task<IEnumerable<DataSourceBase>> GetAllDataSourcesAsync()
        {
            IEnumerable<DataSourceBase> dataSources = await Context.Set<DataSourceBase>().ToListAsync();
            return dataSources.Select(x => x.FromSourceDetails());
        }

        public async Task<DataSourceBase> SaveDataSource(DataSourceBase dataSource) => await UpsertDataSource(dataSource);

        public async Task<DataSourceBase> UpsertDataSource(DataSourceBase source)
        {
            DataSourceBase existing = await this.GetByIdAsync(source.SourceId);
            if (existing == null) Add(source);
            else
            {
                Remove(existing);
                Add(source);
            }
            await Context.SaveChangesAsync();
            return source;
        }
    }
}
