using System.Text.Json.Serialization;

namespace EmissionsMonitorModel.VirtualFileSystem
{
    public class SaveItem
    {
        public SaveItem()
        {
            SaveItemType = SaveItemTypes.SaveItem;
        }

        public int FolderId { get; set; }

        [JsonIgnore]
        public Folder? Folder { get; set; }

        public int SaveItemId { get; set; }

        public string SaveItemName { get; set; }

        public SaveItemTypes SaveItemType { get; set; }
    }

    public enum SaveItemTypes
    {
        SaveItem,
        ExploreSetSaveItem,
        ModelSaveItem
    }
}
