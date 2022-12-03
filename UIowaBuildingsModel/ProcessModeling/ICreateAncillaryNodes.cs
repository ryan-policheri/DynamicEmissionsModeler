namespace EmissionsMonitorModel.ProcessModeling
{
    public interface ICreateAncillaryNodes
    {
        ICollection<ProcessNode> GetAncillaryNodes();
    }
}