using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.DataSources;
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

        //ROOT FOLDERS
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

        //FOLDERS
        public async Task<Folder> GetFolderAsync(int folderId)
        {
            return await _context.Set<Folder>().AsNoTracking()
                .Where(x => x.FolderId == folderId)
                .Include(x => x.ChildFolders)
                .Include(x => x.SaveItems)
                .FirstAsync();
        }

        public async Task<Folder> GetFolderRecursiveAsync(int folderId)
        {
            return _context.Set<Folder>()
                .Include(x => x.SaveItems)
                .AsEnumerable()
                .Where(x => x.FolderId == folderId)
                .ToList()
                .First();
        }

        public async Task<Folder> SaveFolderAsync(Folder folder) => await UpsertFolder(folder);

        public async Task<Folder> UpsertFolder(Folder source)
        {
            Folder existing = await _context.Set<Folder>().FirstOrDefaultAsync(x => x.FolderId == source.FolderId);
            if (existing == null) _context.Set<Folder>().Add(source);
            else
            {
                _context.Set<Folder>().Remove(existing);
                _context.Set<Folder>().Add(source);
            }
            await _context.SaveChangesAsync();
            return source;
        }

        public async Task<Folder> DeleteFolderAsync(int folderId)
        {
            var folder = await _context.Set<Folder>()
                .Where(x => x.FolderId == folderId).FirstAsync();
            _context.Remove(folder);
            await _context.SaveChangesAsync();
            return folder;
        }

        //GENERIC SAVE ITEMS
        public async Task<SaveItem> GetSaveItemAsync(int itemId)
        {
            var item = await _context.Set<SaveItem>().AsNoTracking()
                .Where(x => x.SaveItemId == itemId)
                .FirstAsync();
            return item;
        }

        public async Task<SaveItem> SaveSaveItemInfo(SaveItem item) => await UpsertSaveItem(item);

        public async Task<SaveItem> UpsertSaveItem(SaveItem source)
        {
            SaveItem existing = await _context.Set<SaveItem>().FirstOrDefaultAsync(x => x.SaveItemId == source.SaveItemId);
            if (existing == null) _context.Set<SaveItem>().Add(source);
            else
            {
                _context.Set<SaveItem>().Remove(existing);
                _context.Set<SaveItem>().Add(source);
            }
            await _context.SaveChangesAsync();
            return source;
        }

        public async Task<SaveItem> DeleteSaveItemAsync(int itemId)
        {
            var saveItem = await _context.Set<SaveItem>()
                .Where(x => x.SaveItemId == itemId).FirstAsync();
            _context.Remove(saveItem);
            await _context.SaveChangesAsync();
            return saveItem;
        }

        //IMPLEMENTED SAVE ITEMS
        public async Task<ExploreSetSaveItem> GetExploreSetItemAsync(int itemId)
        {
            var item = await _context.Set<ExploreSetSaveItem>().AsNoTracking()
                .Where(x => x.SaveItemId == itemId)
                .FirstAsync();
            return item;
        }

        public async Task<ExploreSetSaveItem> CreateExploreSetItemAsync(ExploreSetSaveItem saveItem)
        {
            _context.Set<ExploreSetSaveItem>().Add(saveItem);
            await _context.SaveChangesAsync();
            return saveItem;
        }
    }
}
