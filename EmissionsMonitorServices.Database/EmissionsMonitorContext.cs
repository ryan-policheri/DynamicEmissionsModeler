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
            modelBuilder.Entity<EiaDataSource>()
                .HasKey(x => x.SubscriptionKey);
            modelBuilder.Entity<EiaDataSource>().ToTable("DATA_SOURCE_EIA");
        }
    }
}