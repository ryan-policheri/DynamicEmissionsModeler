using System.Diagnostics.CodeAnalysis;

namespace EmissionsMonitorModel.VirtualFileSystem
{
    public class Folder
    {
        public FileSystem FileSystem { get; set; }

        public Folder? ParentFolder { get; set; }

        public int FolderId { get; set; }

        public string FolderName { get; set; }

        public ICollection<Folder>? Folders { get; set; }

        public ICollection<SaveItem>? SaveItems { get; set; }
    }
}