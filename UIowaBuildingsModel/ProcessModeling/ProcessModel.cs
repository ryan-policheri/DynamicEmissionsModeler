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
            if (node.Id < 1) node.Id = ProcessNodes.Count == 0 ? 1 : ProcessNodes.Max(x => x.Id) + 1;
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
                results.Add(node.ToSpec());
            }
            return results;
        }
    }
}
