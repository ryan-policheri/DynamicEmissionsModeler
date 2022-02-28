namespace DotNetCommon.MVVM
{
    public interface ILazyTreeItemBackingModel
    {
        string GetId();
        string GetItemName();
        bool IsKnownLeaf();
    }
}
