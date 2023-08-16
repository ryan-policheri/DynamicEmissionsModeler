using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmissionsMonitorDataAccess.FileSystem
{
    public interface IEmissionsMonitorLocalConfig
    {
        public string AppDataDirectory { get; set; }
    }
}
