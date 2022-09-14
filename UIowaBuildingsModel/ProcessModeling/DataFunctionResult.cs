using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class DataFunctionResult
    {
        public double TotalValue { get; set; }

        public string Unit { get; set; }

        public string UnitForm { get; set; }

        public Func<object, double> ValueRenderFunction { get; set; }
    }
}
