using System.Text.Json.Serialization;

namespace EmissionsMonitorModel.VirtualFileSystem
{
    public class SaveItem
    {
        public int FolderId { get; set; }

        [JsonIgnore]
        public Folder? Folder { get; set; }

        public string SaveItemName { get; set; }

        public int SaveItemId { get; set; }
    }
}
