using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmissionsMonitorModel.ProcessModeling;

namespace EmissionsMonitorModel
{
    public class ProcessModel
    {
        public ICollection<ExchangeNode> ExchangeNodes { get; }

        public ProcessModel AddExchangeNode(ExchangeNode node)
        {
            ExchangeNodes.Add(node);
            return this;
        }
    }
}
