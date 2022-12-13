namespace EmissionsMonitorModel.VirtualFileSystem
{
    public class ExploreSetSaveItem : SaveItem
    {
        public ExploreSetSaveItem()
        {
            SaveItemType = SaveItemTypes.ExploreSetSaveItem;
        }

        public string ExploreSetJsonDetails { get; set; }
    }
}
