using System.Collections.Generic;
using System.Collections.ObjectModel;
using EmissionsMonitorModel.ProcessModeling;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class LikeTermAggregatorNodeViewModel : ProcessNodeViewModel
    {
        private LikeTermsAggregatorNode _model;

        public LikeTermAggregatorNodeViewModel(RobustViewModelDependencies facade) : base(facade)
        {
            AvailableNodeOutputSpecs = new ObservableCollection<NodeOutputSpec>();
        }

        public override string NodeTypeName => "Like Term Aggregator Node";

        public void Load(ProcessNode aggregatorNode, ICollection<NodeOutputSpec> availableSpecs)
        {
            base.Load(aggregatorNode);
            _model = aggregatorNode as LikeTermsAggregatorNode;
            
            AvailableNodeOutputSpecs.Clear();
            foreach (var spec in availableSpecs)
            {
                AvailableNodeOutputSpecs.Add(spec);
            }
        }

        public ObservableCollection<NodeOutputSpec> AvailableNodeOutputSpecs { get; }
    }
}
