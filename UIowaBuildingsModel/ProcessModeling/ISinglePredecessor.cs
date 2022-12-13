namespace EmissionsMonitorModel.ProcessModeling
{
    public interface ISinglePredecessor
    {
        public ProcessNode PrecedingNode { get; set; }
        public int PrecedingNodeId { get; set; }
    }
}