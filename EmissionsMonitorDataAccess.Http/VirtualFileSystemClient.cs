using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.VirtualFileSystem;
using EmissiosMonitorDataAccess.Http;

namespace EmissionsMonitorDataAccess.Http
{
    public class VirtualFileSystemClient : EmissionsMonitorClient, IVirtualFileSystemRepository
    {
        public VirtualFileSystemClient()
        {
            this.SerializerOptions
        }

        public new void Initialize(IEmissionsMonitorClientConfig config)
        {
            base.Initialize(config);
            this.Client.BaseAddress = new Uri(this.PrepareUri("VirtualFileSystem"));
        }
        
        public async Task<List<Folder>> GetAllRootFoldersAsync()
        {
            return (await this.GetAllAsync<Folder>("roots")).ToList();
        }

        public async Task<Folder> CreateRootFolderAsync(Folder folder)
        {
            return await this.PostAsync<Folder>("roots", folder);
        }

        public async Task<Folder> GetFolderAsync(int folderId)
        {
            return await this.GetAsync<Folder>($"folders/{folderId}");
        }

        public async Task<Folder> GetFolderRecursiveAsync(int folderId)
        {
            return await this.GetAsync<Folder>($"folders/{folderId}/recursive");
        }

        public async Task<Folder> SaveFolderAsync(Folder folder)
        {
            return await this.PostAsync<Folder>("folders", folder);
        }

        public async Task<Folder> DeleteFolderAsync(int folderId)
        {
            return await this.DeleteAsync<Folder>($"folders/{folderId}");
        }

        public async Task<SaveItem> GetSaveItemAsync(int itemId)
        {
            return await this.GetAsync<SaveItem>($"saveitems/{itemId}");
        }

        public async Task<SaveItem> SaveSaveItemInfo(SaveItem item)
        {
            return await this.PostAsync<SaveItem>($"saveitems", item);
        }

        public async Task<SaveItem> DeleteSaveItemAsync(int itemId)
        {
            return await this.DeleteAsync<SaveItem>($"saveitems/{itemId}");
        }

        public async Task<ExploreSetSaveItem> GetExploreSetItemAsync(int itemId)
        {
            return await this.GetAsync<ExploreSetSaveItem>($"exploresets/{itemId}");
        }

        public async Task<ExploreSetSaveItem> CreateExploreSetItemAsync(ExploreSetSaveItem saveItem)
        {
            return await this.PostAsync<ExploreSetSaveItem>($"exploresets", saveItem);
        }

        public async Task<ModelSaveItem> GetModelSaveItemAsync(int id)
        {
            return await this.GetAsync<ModelSaveItem>($"processmodels/{id}");
        }

        public async Task<ModelSaveItem> SaveModelSaveItemAsync(ModelSaveItem saveItem)
        {
            return await this.PostAsync<ModelSaveItem>($"processmodels", saveItem);
        }
    }
}