namespace EmissionsMonitorModel.VirtualFileSystem
{
    public class ModelSaveItem : SaveItem
    {
        public ModelSaveItem()
        {
            SaveItemType = SaveItemTypes.ModelSaveItem;
        }

        public string ProcessModelJsonDetails { get; set; }
    }
}
