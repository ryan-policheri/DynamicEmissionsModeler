using EmissionsMonitorModel.DataSources;
using EmissionsMonitorModel.Experiments.DailyCarbonTrend;
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

        public DbSet<DailyCarbonExperiment> DailyCarbonExperiments { get; set; }

        public DbSet<DailyCarbonExperimentRecord> DailyCarbonRecords { get; set; }

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
            saveItemSpec.HasDiscriminator(x => x.SaveItemType)
                .HasValue<SaveItem>(SaveItemTypes.SaveItem)
                .HasValue<ExploreSetSaveItem>(SaveItemTypes.ExploreSetSaveItem)
                .HasValue<ModelSaveItem>(SaveItemTypes.ModelSaveItem);
            saveItemSpec.ToTable("SAVE_ITEM");

            var dailyCarbonExperimentSpec = modelBuilder.Entity<DailyCarbonExperiment>();
            dailyCarbonExperimentSpec.HasKey(x => x.ExperimentId);
            dailyCarbonExperimentSpec.Property(typeof(string), "NodeIdsString");
            dailyCarbonExperimentSpec.Property(typeof(DateTimeOffset), "ExperimentDate");
            dailyCarbonExperimentSpec.ToTable("DAILY_CARBON_EXPERIMENT");
            
            var dailyCarbonExperimentRecordSpec = modelBuilder.Entity<DailyCarbonExperimentRecord>();
            dailyCarbonExperimentRecordSpec.HasKey(x => new { x.ExperimentId, x.Date });
            dailyCarbonExperimentRecordSpec.HasOne(x => x.Experiment).WithMany(x => x.ExperimentRecords).HasForeignKey(x => x.ExperimentId);
            dailyCarbonExperimentRecordSpec.ToTable("DAILY_CARBON_EXPERIMENT_RECORD");
        }
    }
}