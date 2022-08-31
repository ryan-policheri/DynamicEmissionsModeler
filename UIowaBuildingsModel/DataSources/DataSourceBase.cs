using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmissionsMonitorModel.DataSources
{
    public class DataSourceBase
    {
        public DataSourceType SourceType { get; set; }

        public string SourceName { get; set; }
    }


}
