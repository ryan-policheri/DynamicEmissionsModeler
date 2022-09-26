using EmissionsMonitorModel.DataSources;
using Microsoft.EntityFrameworkCore;

namespace EmissionsMonitorDataAccess.Database
{
    public class EmissionsMonitorContext : DbContext
    {
        public EmissionsMonitorContext(DbContextOptions<EmissionsMonitorContext> options) : base(options)
        {
            
        }

        public DbSet<DataSourceBase> DataSources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var spec = modelBuilder.Entity<DataSourceBase>();
            spec.HasKey(x => x.SourceId);
            spec.ToTable("DATA_SOURCE");
        }
    }
}