using DotNetCommon.Extensions;
using EmissionsMonitorModel.DataSources;
using System.Runtime.Serialization;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class ProcessModel
    {
        public ProcessModel()
        {
            ProcessNodes = new List<ProcessNode>();
        }

        public void OnDeserialized()
        {
            //Populate Preceding Nodes object references
            foreach (var node in this.GetAllNodes(true))
            {
                if (node is ISinglePredecessor) ((ISinglePredecessor)node).PrecedingNode = this.GetAllNodes(false).First(x => x.Id == ((ISinglePredecessor)node).PrecedingNodeId);
                if (node is IMultiplePredecessor)
                {
                    var multiPredsNode = (IMultiplePredecessor)node;
                    foreach (int id in multiPredsNode.PrecedingNodeIds)
                    {
                        multiPredsNode.PrecedingNodes.Add(this.GetAllNodes(false).First(x => x.Id == id));
                    }
                }
                if (node.GetType() == typeof(MultiSplitterNode))
                {
                    MultiSplitterNode multiSplitterNode = (MultiSplitterNode)node;
                    multiSplitterNode.OnChildNodeDynamicallyAdded += (splitterNode, childNode) =>
                    {//Smelly. The multi splitter node is going to dynamically add nodes based on the user-entered split functions.
                     //This listener is here so that when a node gets dyanmically the model can assign its ID
                        childNode.Id = GetAllNodes(true).Max(x => x.Id) + 1;
                    };

                    foreach (MultiSplitResultNode splitResult in multiSplitterNode.SplitResultNodes)
                    {
                        splitResult.OwningSplitter = multiSplitterNode;
                    }
                }
            }
        }

        public string ModelName { get; set; }

        public ICollection<ProcessNode> ProcessNodes { get; set; }

        public void AddProcessNode(ProcessNode node)
        {
            if (node.Id < 1)
            {
                node.Id = ProcessNodes.Count == 0 ? 1 : GetAllNodes(true).Max(x => x.Id) + 1;
                if (node.GetType() == typeof(StreamSplitterNode))
                {
                    StreamSplitterNode splitterNode = (StreamSplitterNode)node;
                    splitterNode.SplitResultNodeId = node.Id + 1;
                    splitterNode.RemainderResultNodeId = node.Id + 2;
                }
                if (node.GetType() == typeof(MultiSplitterNode))
                {
                    MultiSplitterNode multiSplitterNode = (MultiSplitterNode)node;
                    multiSplitterNode.RemainderResultNodeId = node.Id + 1;
                    multiSplitterNode.OnChildNodeDynamicallyAdded += (splitterNode, childNode) =>
                    {//Smelly. The multi splitter node is going to dynamically add nodes based on the user-entered split functions.
                     //This listener is here so that when a node gets dyanmically the model can assign its ID
                        childNode.Id = GetAllNodes(true).Max(x => x.Id) + 1;
                    };
                }
            }
            ProcessNodes.Add(node);
        }

        /// <summary>
        /// Gathers all the process nodes contained in the process model, including nodes that are dynamically created by other nodes (I.E. the result and remainder of a splitter node).
        /// </summary>
        /// <param name="includeAncillaryRoots">If true the nodes that manage ancillary nodes will be included otherwise only the ancillary nodes will be included.</param>
        public ICollection<ProcessNode> GetAllNodes(bool includeAncillaryRoots = false)
        {
            List<ProcessNode> results = new List<ProcessNode>();
            foreach (ProcessNode node in ProcessNodes)
            {
                if (node is ICreateAncillaryNodes)
                {
                    results.AddRange(((ICreateAncillaryNodes)node).GetAncillaryNodes());
                    if (includeAncillaryRoots) results.Add(node); 
                }
                else results.Add(node);
            }

            return results;
        }

        public ICollection<NodeOutputSpec> GetAllNodeSpecs(bool includeAncillaryRoots = false)
        {
            return GetAllNodes(includeAncillaryRoots).Select(x => x.ToSpec()).ToList();
        }

        public ICollection<DataSourceSeriesUri> GetAllUniqueSeriesUris(IEnumerable<int>? nodeIds = null)
        {//Get a copy of all uniquely identified data streams.
            var processNodes = nodeIds == null ? this.ProcessNodes : this.ProcessNodes.Where(x => nodeIds.Any(y => x.Id == y));
            return processNodes
                .SelectMany(x => x.GetUserDefinedFunctions())
                .SelectMany(x => x.FunctionFactors)
                .Select(x => x.FactorUri.Copy())
                .GroupBy(x => x.EquivelentSeriesAndConfigJoinKey(true))
                .Select(x => x.First())
                .ToList();
        }
    }
}
