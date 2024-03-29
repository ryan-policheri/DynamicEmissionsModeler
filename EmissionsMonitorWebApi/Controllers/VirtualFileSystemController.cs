﻿using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.VirtualFileSystem;
using Microsoft.AspNetCore.Mvc;

namespace EmissionsMonitorWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VirtualFileSystemController : ControllerBase
    {
        private readonly IVirtualFileSystemRepository _repo;
        private readonly ILogger<VirtualFileSystemController> _logger;

        public VirtualFileSystemController(IVirtualFileSystemRepository repo, ILogger<VirtualFileSystemController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet("roots")]
        public async Task<ICollection<Folder>> GetAllRootFolders()
        {
            return await _repo.GetAllRootFoldersAsync();
        }

        [HttpPost("roots")] //TODO: Require authorization
        public async Task<Folder> CreateRootFolder(Folder folder)
        {
            return await _repo.CreateRootFolderAsync(folder);
        }


        [HttpGet("folders/{folderId}")]
        public async Task<Folder> GetFolder(int folderId)
        {
            return await _repo.GetFolderAsync(folderId);
        }

        [HttpGet("folders/{folderId}/recursive")]
        public async Task<Folder> GetFolderRecursiveAsync(int folderId)
        {
            return await _repo.GetFolderRecursiveAsync(folderId);
        }

        [HttpPost("folders")]
        public async Task<Folder> SaveFolderAsync(Folder folder)
        {
            return await _repo.SaveFolderAsync(folder);
        }

        [HttpDelete("folders/{folderId}")]
        public async Task<Folder> DeleteFolderAsync(int folderId)
        {
            return await _repo.DeleteFolderAsync(folderId);
        }


        [HttpGet("saveitems/{itemId}")]
        public async Task<SaveItem> GetSaveItemAsync(int itemId)
        {
            return await _repo.GetSaveItemAsync(itemId);
        }

        [HttpPost("saveitems")]
        public async Task<SaveItem> SaveSaveItemAsync(SaveItem saveItem)
        {
            return await _repo.SaveSaveItemInfo(saveItem);
        }

        [HttpDelete("saveitems/{saveItemId}")]
        public async Task<SaveItem> DeleteSaveItemAsync(int saveItemId)
        {
            return await _repo.DeleteSaveItemAsync(saveItemId);
        }


        [HttpGet("exploresets/{itemId}")]
        public async Task<ExploreSetSaveItem> GetExploreSetItemAsync(int itemId)
        {
            return await _repo.GetExploreSetItemAsync(itemId);
        }

        [HttpPost("exploresets")]
        public async Task<ExploreSetSaveItem> CreateExploreSetItemAsync(ExploreSetSaveItem exploreSetItem)
        {
            return await _repo.CreateExploreSetItemAsync(exploreSetItem);
        }

        [HttpGet("processmodels/{itemId}")]
        public async Task<ModelSaveItem> GetProcessModelAsync(int itemId)
        {
            return await _repo.GetModelSaveItemAsync(itemId);
        }

        [HttpPost("processmodels")]
        public async Task<ModelSaveItem> SaveProcessModelAsync(ModelSaveItem modelSaveItem)
        {
            return await _repo.SaveModelSaveItemAsync(modelSaveItem);
        }
    }
}
