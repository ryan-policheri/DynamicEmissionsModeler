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
    }
}
