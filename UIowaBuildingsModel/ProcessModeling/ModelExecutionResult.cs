using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmissionsMonitorModel.TimeSeries;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class ModelExecutionResult
    {
        public ICollection<MonitorSeries> Stuff = new List<MonitorSeries>();
    }
}
