using EmissionsMonitorModel.DataSources;
using EmissionsMonitorModel.TimeSeries;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class ProcessModel
    {
        public ProcessModel()
        {
            ProcessNodes = new List<ProcessNode>();
        }

        public string ModelName { get; set; }

        public ICollection<ProcessNode> ProcessNodes { get; set; }

        public void AddProcessNode(ProcessNode node)
        {
            if (node.Id < 1)
            {
                node.Id = ProcessNodes.Count == 0 ? 1 : GetAllNodes().Max(x => x.Id) + 1;
                if (node.GetType() == typeof(StreamSplitterNode))
                {
                    StreamSplitterNode splitterNode = (StreamSplitterNode)node;
                    splitterNode.SplitResultNodeId = node.Id + 1;
                    splitterNode.RemainderResultNodeId = node.Id + 2;
                }
                if (node.GetType() == typeof(MultiSplitterNode))
                {
                    MultiSplitterNode multiSplitterNode = (MultiSplitterNode)node;
                    multiSplitterNode.RemainderResultNode.Id = node.Id;
                    multiSplitterNode.OnChildNodeDynamicallyAdded += (splitterNode, childNode) => //Smelly
                    {
                        childNode.Id = GetAllNodes().Max(x => x.Id) + 1;
                    };
                }
            }
            ProcessNodes.Add(node);
        }

        public ICollection<DataSourceSeriesUri> GetAllSeriesUris()
        {
            ICollection<DataSourceSeriesUri> seriesUris = new List<DataSourceSeriesUri>();

            foreach (ProcessNode node in this.ProcessNodes)
            {
                foreach (DataFunction function in node.GetUserDefinedFunctions())
                {
                    foreach (FunctionFactor factor in function.FunctionFactors)
                    {
                        if(!seriesUris.Any(x => x.Uri == factor.FactorUri.Uri)) seriesUris.Add(factor.FactorUri);
                    }
                }
            }

            return seriesUris;
        }

        public ICollection<ProcessNode> GetAllNodes(bool includeSplitterRoots = false)
        {
            List<ProcessNode> results = new List<ProcessNode>();
            foreach (ProcessNode node in ProcessNodes)
            {
                if (node.GetType() == typeof(StreamSplitterNode)) //Creates 2 child nodes (split and remainder)
                {
                    var splitterNode = (StreamSplitterNode)node;
                    if(includeSplitterRoots) results.Add(splitterNode);
                    results.Add(splitterNode.SplitResultNode);
                    results.Add(splitterNode.RemainderResultNode);
                }
                else if (node.GetType() == typeof(MultiSplitterNode)) //Creates n child nodes (1 to many splits and 1 remainder)
                {
                    var multiSplitterNode = (MultiSplitterNode)node;
                    if (includeSplitterRoots) results.Add(multiSplitterNode);
                    foreach(var child in multiSplitterNode.SplitResultNodes) results.Add(child);
                    results.Add(multiSplitterNode.RemainderResultNode);
                }
                else
                {
                    results.Add(node);
                }
            }

            return results;
        }

        public ICollection<NodeOutputSpec> GetAllNodeSpecs(bool includeSplitterRoots = false)
        {
            return GetAllNodes(includeSplitterRoots).Select(x => x.ToSpec()).ToList();
        }
    }
}
