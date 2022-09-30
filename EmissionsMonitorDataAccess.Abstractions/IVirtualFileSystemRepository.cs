using EmissionsMonitorModel.VirtualFileSystem;

namespace EmissionsMonitorDataAccess.Abstractions
{
    public interface IVirtualFileSystemRepository 
    {
        public Task<ICollection<FileSystem>> GetAllFileSystems();

        public Task<FileSystem> GetFileSystemByIdAsync(int fileSystemId);

        public Task<FileSystem> AddFileSystemAsync(FileSystem fileSystem);

        public Task<Folder> AddFolderAsync(Folder folder);
    }
}
