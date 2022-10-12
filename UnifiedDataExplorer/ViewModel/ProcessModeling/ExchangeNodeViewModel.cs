using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmissionsMonitorModel.ProcessModeling;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class ExchangeNodeViewModel : ProcessNodeViewModel
    {
        private readonly ExchangeNode _model;

        public ExchangeNodeViewModel(ExchangeNode exchangeNode) : base(exchangeNode)
        {
            _model = exchangeNode;
        }
    }
}
