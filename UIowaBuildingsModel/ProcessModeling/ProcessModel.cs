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
                node.Id = ProcessNodes.Count == 0 ? 1 : ProcessNodes.Max(x => x.Id) + 1;
                if (node.GetType() == typeof(StreamSplitterNode))
                {
                    StreamSplitterNode splitterNode = (StreamSplitterNode)node;
                    splitterNode.SplitResultNodeId = node.Id;
                    splitterNode.RemainderResultNodeId = node.Id + 1;
                    node.Id += 2; //Make this node id bigger than the other node ids so that the max statement above still works
                }
            }
            ProcessNodes.Add(node);
        }

        public IEnumerable<ProcessNode> GetProcessLeafs()
        {
            //TODO: Implement this correctly
            return ProcessNodes;
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

        public ICollection<NodeOutputSpec> CalculateDerivedSeries()
        {
            List<NodeOutputSpec> results = new List<NodeOutputSpec>();
            foreach (ProcessNode node in ProcessNodes)
            {
                if (node.GetType() != typeof(StreamSplitterNode)) results.Add(node.ToSpec());
                else
                {
                    var splitterNode = (StreamSplitterNode)node;
                    results.Add(splitterNode.SplitResultNode.ToSpec());
                    results.Add(splitterNode.RemainderResultNode.ToSpec());
                }
            }
            return results;
        }
    }
}
