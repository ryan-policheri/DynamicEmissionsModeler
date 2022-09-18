namespace EmissionsMonitorModel.ProcessModeling
{
    public class ProcessModel
    {
        public string ModelName { get; set; }

        public IEnumerable<ProcessNode> ProcessNodes { get; set; }
    }
}
