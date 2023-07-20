using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DotNetCommon.Helpers;

namespace EmissionsMonitorDataAccess.Database
{
    internal class EmissionsMonitorContextFactory : IDesignTimeDbContextFactory<EmissionsMonitorContext>
    {
        public EmissionsMonitorContext CreateDbContext(string[] args)
        {
            //if (!System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Launch();
            DesignTimeConfig config = CommandLineHelpers.BindArgumentsToType<DesignTimeConfig>(args);
            DbContextOptionsBuilder <EmissionsMonitorContext> options = new DbContextOptionsBuilder<EmissionsMonitorContext>();
            options.UseSqlServer(config.BuildConnectionString());
            return new EmissionsMonitorContext(options.Options);
        }
    }
}
