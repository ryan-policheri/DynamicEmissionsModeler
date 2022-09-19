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
            ProcessNodes.Add(node);
        }

        public IEnumerable<ProcessNode> GetProcessLeafs()
        {
            //TODO: Implement this correctly
            return ProcessNodes;
        }

        public ICollection<string> GetAllSeriesIds()
        {
            ICollection<string> seriesIds = new List<string>();

            foreach (ProcessNode node in this.ProcessNodes)
            {
                foreach (DataFunction function in node.GetUserDefinedFunctions())
                {
                    foreach (FunctionFactor factor in function.FunctionFactors)
                    {
                        if(!seriesIds.Any(x => x == factor.FactorUri)) seriesIds.Add(factor.FactorUri);
                    }
                }
            }

            return seriesIds;
        }
    }
}
