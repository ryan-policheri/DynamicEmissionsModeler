using EmissionsMonitorModel.TimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmissionsMonitorModel.ProcessModeling
{
    public abstract class ProcessNode
    {
        public string Name { get; set; }

        public abstract ICollection<DataFunction> GetUserDefinedFunctions();

        public abstract ProductCostResults RenderProductAndCosts(ICollection<DataPoint> dataPoints);
    }
}
