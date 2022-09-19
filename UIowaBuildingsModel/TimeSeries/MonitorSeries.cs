using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmissionsMonitorModel.TimeSeries
{
    public class MonitorSeries : Series
    {
        public new ICollection<MonitorDataPoint> DataPoints { get; set; }
    }
}
