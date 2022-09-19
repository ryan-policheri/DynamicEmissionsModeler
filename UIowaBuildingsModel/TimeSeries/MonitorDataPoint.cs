using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmissionsMonitorModel.ProcessModeling;

namespace EmissionsMonitorModel.TimeSeries
{
    public class MonitorDataPoint
    {
        public DateTimeOffset Timestamp { get; set; }
        
        public ProductCostResults Values { get; set; }

    }
}
