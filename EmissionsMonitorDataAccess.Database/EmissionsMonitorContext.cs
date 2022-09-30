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

        public DbSet<FileSystem> FileSystems { get; set; }

        public DbSet<Folder> Folders { get; set; }

        public DbSet<SaveItem> SaveItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var dataSourceSpec = modelBuilder.Entity<DataSourceBase>();
            dataSourceSpec.HasKey(x => x.SourceId);
            dataSourceSpec.ToTable("DATA_SOURCE");

            var fileSystemSpec = modelBuilder.Entity<FileSystem>();
            fileSystemSpec.HasKey(x => x.FileSystemId);
            fileSystemSpec.ToTable("FILE_SYSTEM");
            //var exploreSetFileSystem = new FileSystem { FileSystemId = 1, FileSystemName = "Explore Sets" };
            //var fileSystems = new List<FileSystem>() { exploreSetFileSystem };
            //modelBuilder.Entity<FileSystem>().HasData(fileSystems);

            var folderSpec = modelBuilder.Entity<Folder>();
            folderSpec.HasKey(x => x.FolderId);
            folderSpec.Property<int>("OwningFileSystemId");
            folderSpec
                .HasOne(x => x.FileSystem)
                .WithMany(x => x.Folders)
                .HasForeignKey("OwningFileSystemId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            folderSpec
                .HasOne(x => x.ParentFolder)
                .WithMany(x => x.Folders)
                .HasForeignKey("ParentFolderId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            folderSpec.ToTable("FOLDER");
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