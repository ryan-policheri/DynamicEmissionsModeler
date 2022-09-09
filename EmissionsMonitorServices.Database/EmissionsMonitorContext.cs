using EmissionsMonitorModel.DataSources;
using Microsoft.EntityFrameworkCore;

namespace EmissionsMonitorServices.Database
{
    public class EmissionsMonitorContext : DbContext
    {
        public EmissionsMonitorContext(DbContextOptions<EmissionsMonitorContext> options) : base(options)
        {
            
        }

        public DbSet<EiaDataSource> EiaDataSources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var spec = modelBuilder.Entity<EiaDataSource>();
            spec.HasKey(x => x.SubscriptionKey);
            spec.Property(x => x.BaseUrl);
            spec.ToTable("DATA_SOURCE_EIA");
        }
    }
}