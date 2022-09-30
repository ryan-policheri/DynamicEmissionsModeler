using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace EmissionsMonitorModel.VirtualFileSystem
{
    public class FileSystem
    {
        public int FileSystemId { get; set; }

        public string FileSystemName { get; set; }

        public ICollection<Folder>? Folders { get; set; }

        public ICollection<SaveItem>? SaveItems { get; set; }
    }
}
