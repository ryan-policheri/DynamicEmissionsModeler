using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedDataExplorer.ViewModel
{
    public class StatViewModel
    {
        public StatViewModel(string title, double value, string units)
        {
            Title = title;
            if(value > 1) Value = String.Format("{0:n2}", value);
            else Value = Math.Round(value, 4).ToString();
            Units = units;
        }

        public string Title { get; set; }

        public string Value { get; set; }

        public string Units { get; set; }
    }
}
