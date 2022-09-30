using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.VirtualFileSystem;
using Microsoft.EntityFrameworkCore;

namespace EmissionsMonitorDataAccess.Database.Repositories
{
    public class VirtualFileSystemRepository : IVirtualFileSystemRepository
    {
        private readonly EmissionsMonitorContext _context;

        public VirtualFileSystemRepository(EmissionsMonitorContext context)
        {
            _context = context;
        }

        public async Task<ICollection<FileSystem>> GetAllFileSystems()
        {
            return await _context.Set<FileSystem>().ToListAsync();
        }

        public async Task<FileSystem> GetFileSystemByIdAsync(int fileSystemId)
        {
            return await _context.Set<FileSystem>()
                .Include(x => x.Folders)
                .FirstAsync();
        }
        public async Task<FileSystem> AddFileSystemAsync(FileSystem fileSystem)
        {
            _context.Set<FileSystem>().Add(fileSystem);
            await _context.SaveChangesAsync();
            return fileSystem;
        }

        public async Task<Folder> AddFolderAsync(Folder folder)
        {
            FileSystem owningFileSystem = await GetFileSystemByIdAsync(folder.FileSystem.FileSystemId);
            folder.FileSystem = owningFileSystem;
            if (folder.ParentFolder != null && folder.ParentFolder.FolderId > 0)
            {
                var parent = await _context.Set<Folder>().FirstAsync(x => x.FolderId == folder.ParentFolder.FolderId);
                folder.ParentFolder = parent;
            }
            _context.Set<Folder>().Add(folder);
            await _context.SaveChangesAsync();
            folder.FileSystem = null;
            folder.ParentFolder = null;
            return folder;
        }
    }
}
