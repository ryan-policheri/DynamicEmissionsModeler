namespace EmissionsMonitorModel.ProcessModeling
{
    public interface IMultiplePredecessor
    {
        public List<int> PrecedingNodeIds { get; set; }
        public List<ProcessNode> PrecedingNodes { get; set; }
    }
}