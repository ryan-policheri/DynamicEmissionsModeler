using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmissionsMonitorServices.Database
{
    public static class DbInitializer
    {
        public static void Initialize(EmissionsMonitorContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
