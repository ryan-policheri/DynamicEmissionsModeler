using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmissionsMonitorModel.ProcessModeling;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class LikeTermAggregatorNodeViewModel : ProcessNodeViewModel
    {
        private readonly LikeTermsAggregatorNode _model;

        public LikeTermAggregatorNodeViewModel(LikeTermsAggregatorNode aggregatorNode) : base(aggregatorNode)
        {
            _model = aggregatorNode;
        }

        public override string NodeTypeName => "Like Term Aggregator Node";
    }
}
