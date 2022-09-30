using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.VirtualFileSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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

        [HttpGet]
        public async Task<ICollection<FileSystem>> GetFileSystems()
        {
            return await _repo.GetAllFileSystems();
        }

        [HttpGet("{fileSystemId}")]
        public async Task<FileSystem> GetFileSystem(int fileSystemId)
        {
            return await _repo.GetFileSystemByIdAsync(fileSystemId);
        }

        [HttpPost]
        public async Task<FileSystem> AddFileSystem(FileSystem fileSystem)
        {
            return await _repo.AddFileSystemAsync(fileSystem);
        }

        [HttpPost("folder")]
        public async Task<Folder> AddFolder([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] Folder folder)
        {
            return await _repo.AddFolderAsync(folder);
        }
    }
}
