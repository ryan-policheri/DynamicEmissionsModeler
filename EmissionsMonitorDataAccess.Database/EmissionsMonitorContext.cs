using EmissionsMonitorModel.DataSources;
using EmissionsMonitorModel.Experiments.DailyCarbonTrend;
using EmissionsMonitorModel.Experiments.IndStudyExp;
using EmissionsMonitorModel.Experiments.NodeInspect;
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

        public DbSet<IndStudyExperiment> IndStudyExperiments { get; set; }

        public DbSet<NodeInspectExperiment> NodeInspectExperiments { get; set; }

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

            var indStudyExpSpec = modelBuilder.Entity<IndStudyExperiment>();
            indStudyExpSpec.HasKey(x => x.ExperimentId);
            indStudyExpSpec.Property(typeof(DateTimeOffset), "ExperimentDate");
            indStudyExpSpec.ToTable("IND_STUDY_EXPERIMENT");

            var indStudyExpRecordSpec = modelBuilder.Entity<IndStudyExperimentRecord>();
            indStudyExpRecordSpec.HasKey(x => new { x.ExperimentId, x.Timestamp });
            indStudyExpRecordSpec.HasOne(x => x.Experiment).WithMany(x => x.ExperimentRecords).HasForeignKey(x => x.ExperimentId);
            indStudyExpRecordSpec.ToTable("IND_STUDY_EXPERIMENT_RECORD");

            var nodeInspectExpSpec = modelBuilder.Entity<NodeInspectExperiment>();
            nodeInspectExpSpec.HasKey(x => x.ExperimentId);
            nodeInspectExpSpec.Property(typeof(DateTimeOffset), "ExperimentDate");
            nodeInspectExpSpec.ToTable("NODE_INSPECT_EXPERIMENT");

            var nodeInspectExpRecordSpec = modelBuilder.Entity<NodeInspectExperimentRecord>();
            nodeInspectExpRecordSpec.HasKey(x => new { x.ExperimentId, x.Timestamp });
            nodeInspectExpRecordSpec.HasOne(x => x.Experiment).WithMany(x => x.ExperimentRecords).HasForeignKey(x => x.ExperimentId);
            nodeInspectExpRecordSpec.ToTable("NODE_INSPECT_EXPERIMENT_RECORD");
        }
    }
}