namespace EmissionsMonitorModel.VirtualFileSystem
{
    public class Folder
    {
        public int? ParentFolderId { get; set; }

        public int FolderId { get; set; }

        public string FolderName { get; set; }

        public List<Folder>? ChildFolders { get; set; }

        public List<SaveItem>? SaveItems { get; set; }
    }
}