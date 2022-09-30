using EmissionsMonitorModel.DataSources;
using EmissionsMonitorModel.VirtualFileSystem;
using Microsoft.EntityFrameworkCore;

namespace EmissionsMonitorDataAccess.Database
{
    public class EmissionsMonitorContext : DbContext
    {
        public EmissionsMonitorContext(DbContextOptions<EmissionsMonitorContext> options) : base(options)
        {
            
        }

        public DbSet<DataSourceBase> DataSources { get; set; }

        public DbSet<Folder> Folders { get; set; }

        public DbSet<SaveItem> SaveItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var dataSourceSpec = modelBuilder.Entity<DataSourceBase>();
            dataSourceSpec.HasKey(x => x.SourceId);
            dataSourceSpec.ToTable("DATA_SOURCE");
            
            var folderSpec = modelBuilder.Entity<Folder>();
            folderSpec.HasKey(x => x.FolderId);
            folderSpec
                .HasOne<Folder>()
                .WithMany(x => x.ChildFolders)
                .HasForeignKey("ParentFolderId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            folderSpec.ToTable("FOLDER");

            var saveItemSpec = modelBuilder.Entity<SaveItem>();
            saveItemSpec.HasKey(x => x.SaveItemId);
            saveItemSpec
                .HasOne(x => x.Folder)
                .WithMany(x => x.SaveItems)
                .HasForeignKey(x => x.FolderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            saveItemSpec.ToTable("SAVE_ITEM");
            //var ryanFolder = new Folder { FileSystem = exploreSetFileSystem, FolderId = 1, FolderName = "Ryan's Folder" };
            //var folders = new List<Folder>()
            //{
            //    ryanFolder,
            //    new Folder { FileSystem = exploreSetFileSystem, FolderId = 2, FolderName = "Another Folder" },
            //    new Folder { FileSystem = exploreSetFileSystem, FolderId = 3, FolderName = "A sub folder", ParentFolder = ryanFolder }
            //};
            //modelBuilder.Entity<Folder>().HasData(folders);


        }
    }
}