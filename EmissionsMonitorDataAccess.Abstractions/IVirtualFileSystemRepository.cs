using EmissionsMonitorModel.VirtualFileSystem;

namespace EmissionsMonitorDataAccess.Abstractions
{
    public interface IVirtualFileSystemRepository
    {
        public Task<List<Folder>> GetAllRootFoldersAsync();

        public Task<Folder> CreateRootFolderAsync(Folder folder);

        public Task<Folder> GetFolderAsync(int folderId);

        public Task<Folder> GetFolderWithContents(int folderId);

        public Task<Folder> CreateFolderAsync(Folder folder);

        public Task<SaveItem> CreateSaveItemAsync(SaveItem item);
    }
}
