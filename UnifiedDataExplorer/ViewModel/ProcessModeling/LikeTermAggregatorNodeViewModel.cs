using EmissionsMonitorModel.ProcessModeling;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class LikeTermAggregatorNodeViewModel : ProcessNodeViewModel
    {
        private LikeTermsAggregatorNode _model;

        public LikeTermAggregatorNodeViewModel(RobustViewModelDependencies facade) : base(facade)
        {
        }

        public override string NodeTypeName => "Like Term Aggregator Node";

        public void Load(LikeTermsAggregatorNode aggregatorNode)
        {
            base.Load(aggregatorNode);
            _model = aggregatorNode;
        }
    }
}
