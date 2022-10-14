using EmissionsMonitorModel.VirtualFileSystem;

namespace EmissionsMonitorDataAccess.Abstractions
{
    public interface IVirtualFileSystemRepository
    {
        public Task<List<Folder>> GetAllRootFoldersAsync();

        public Task<Folder> CreateRootFolderAsync(Folder folder);

        public Task<Folder> GetFolderAsync(int folderId);

        public Task<Folder> GetFolderRecursiveAsync(int folderId);

        public Task<Folder> SaveFolderAsync(Folder folder);
        public Task<Folder> DeleteFolderAsync(int folderId);
        public Task<SaveItem> GetSaveItemAsync(int itemId);
        public Task<SaveItem> SaveSaveItemInfo(SaveItem item);
        public Task<SaveItem> DeleteSaveItemAsync(int itemId);
        public Task<ExploreSetSaveItem> GetExploreSetItemAsync(int itemId);
        public Task<ExploreSetSaveItem> CreateExploreSetItemAsync(ExploreSetSaveItem saveItem);
        public Task<ModelSaveItem> GetModelSaveItemAsync(int id);
        public Task<ModelSaveItem> SaveModelSaveItemAsync(ModelSaveItem saveItem);
    }
}
