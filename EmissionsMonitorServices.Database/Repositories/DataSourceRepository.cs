using EmissionsMonitorModel.DataSources;
using Microsoft.EntityFrameworkCore;

namespace EmissionsMonitorServices.Database.Repositories
{
    public class DataSourceRepository : GenericRepository<DataSourceBase, EmissionsMonitorContext>
    {
        public DataSourceRepository(EmissionsMonitorContext context) : base(context)
        {
        }

        public async Task<IEnumerable<DataSourceBase>> GetAllDataSourcesAsync()
        {
            IEnumerable<DataSourceBase> dataSources = await Context.Set<DataSourceBase>().ToListAsync();
            return dataSources;
        }

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
