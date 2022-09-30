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

        public async Task<List<Folder>> GetAllRootFoldersAsync()
        {
            return await _context.Set<Folder>()
                .AsNoTracking()
                .Where(x => x.ParentFolderId == null)
                .ToListAsync();
        }

        public async Task<Folder> CreateRootFolderAsync(Folder folder)
        {
            folder.ParentFolderId = null;
            _context.Set<Folder>().Add(folder);
            await _context.SaveChangesAsync();
            return folder;
        }

        public async Task<Folder> GetFolderAsync(int folderId)
        {
            return await _context.Set<Folder>().AsNoTracking()
                .Where(x => x.FolderId == folderId).FirstAsync();
        }

        public async Task<Folder> GetFolderWithContents(int folderId)
        {
            return _context.Set<Folder>()
                .Include(x => x.SaveItems)
                .AsEnumerable()
                .Where(x => x.FolderId == folderId)
                .ToList()
                .First();
        }

        public async Task<Folder> CreateFolderAsync(Folder folder)
        {
            _context.Set<Folder>().Add(folder);
            await _context.SaveChangesAsync();
            return folder;
        }

        public async Task<SaveItem> CreateSaveItemAsync(SaveItem saveItem)
        {
            _context.Set<SaveItem>().Add(saveItem);
            await _context.SaveChangesAsync();
            return saveItem;
        }
    }
}
