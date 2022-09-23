using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmissionsMonitorModel.TimeSeries
{
    public class DataPoint
    {
        public Series Series { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public double Value { get; set; }
    }
}
