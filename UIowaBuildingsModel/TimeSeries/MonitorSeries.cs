using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmissionsMonitorModel.TimeSeries
{
    public class MonitorSeries
    {
        public string SeriesName { get; set; }

        public ICollection<MonitorDataPoint> DataPoints { get; set; }
    }
}
