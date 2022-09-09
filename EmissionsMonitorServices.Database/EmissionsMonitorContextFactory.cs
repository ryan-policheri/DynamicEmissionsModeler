using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EmissionsMonitorServices.Database
{
    internal class EmissionsMonitorContextFactory : IDesignTimeDbContextFactory<EmissionsMonitorContext>
    {
        public EmissionsMonitorContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<EmissionsMonitorContext> options = new DbContextOptionsBuilder<EmissionsMonitorContext>();
            options.UseSqlServer("Server=.\\SQLEXPRESS;Database=EmissionsMonitor;Trusted_Connection=True;"); //Use local DB for designing
            return new EmissionsMonitorContext(options.Options);
        }
    }
}
