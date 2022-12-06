using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace UIowaBuildingsConsoleApp.Experiments.Phase1Reporting
{
    public class UsageTime
    {
        public DateTimeOffset Time { get; set; }

        public Mass Co2Mass { get; set; }
    }
}
