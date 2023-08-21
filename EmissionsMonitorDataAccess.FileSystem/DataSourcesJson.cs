using EmissionsMonitorModel.DataSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmissionsMonitorDataAccess.FileSystem
{
    public class DataSourcesJson
    {
        public List<DataSourceBase> DataSources { get; set; } = new List<DataSourceBase>();
    }
}
