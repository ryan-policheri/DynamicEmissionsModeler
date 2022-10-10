using EmissionsMonitorDataAccess.Abstractions;
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
        public async Task<Folder> CreateFolderAsync(Folder folder)
        {
            return await _repo.CreateFolderAsync(folder);
        }

        [HttpDelete("folders/{folderId}")]
        public async Task<Folder> DeleteFolderAsync(int folderId)
        {
            return await _repo.DeleteFolderAsync(folderId);
        }

        [HttpPost("saveitems")]
        public async Task<SaveItem> CreateSaveItemAsync(SaveItem saveItem)
        {
            return await _repo.CreateSaveItemAsync(saveItem);
        }

        [HttpDelete("saveitems/{saveItemId}")]
        public async Task<SaveItem> DeleteSaveItemAsync(int saveItemId)
        {
            return await _repo.DeleteSaveItemAsync(saveItemId);
        }
    }
}
