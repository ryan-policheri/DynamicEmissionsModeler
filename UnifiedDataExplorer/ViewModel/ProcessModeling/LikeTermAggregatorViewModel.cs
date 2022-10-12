using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmissionsMonitorModel.ProcessModeling;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class LikeTermAggregatorViewModel : ProcessNodeViewModel
    {
        private readonly LikeTermsAggregatorNode _model;

        public LikeTermAggregatorViewModel(LikeTermsAggregatorNode aggregatorNode) : base(aggregatorNode)
        {
            _model = aggregatorNode;
        }
    }
}
