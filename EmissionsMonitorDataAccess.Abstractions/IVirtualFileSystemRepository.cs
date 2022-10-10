using EmissionsMonitorModel.VirtualFileSystem;

namespace EmissionsMonitorDataAccess.Abstractions
{
    public interface IVirtualFileSystemRepository
    {
        public Task<List<Folder>> GetAllRootFoldersAsync();

        public Task<Folder> CreateRootFolderAsync(Folder folder);

        public Task<Folder> GetFolderAsync(int folderId);

        public Task<Folder> GetFolderRecursiveAsync(int folderId);

        public Task<Folder> CreateFolderAsync(Folder folder);
        public Task<Folder> DeleteFolderAsync(int folderId);
        public Task<SaveItem> GetSaveItemDetailsAsync(SaveItem item);
        public Task<SaveItem> CreateSaveItemAsync(SaveItem item);
        public Task<SaveItem> DeleteSaveItemAsync(int itemId);
    }
}
