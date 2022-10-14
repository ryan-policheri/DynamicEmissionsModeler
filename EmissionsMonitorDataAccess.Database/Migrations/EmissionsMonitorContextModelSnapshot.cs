﻿// <auto-generated />
using System;
using EmissionsMonitorDataAccess.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EmissionsMonitorDataAccess.Database.Migrations
{
    [DbContext(typeof(EmissionsMonitorContext))]
    partial class EmissionsMonitorContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("EmissionsMonitorModel.DataSources.DataSourceBase", b =>
                {
                    b.Property<int>("SourceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SourceId"), 1L, 1);

                    b.Property<string>("SourceDetailsJson")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SourceName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SourceType")
                        .HasColumnType("int");

                    b.HasKey("SourceId");

                    b.ToTable("DATA_SOURCE", (string)null);
                });

            modelBuilder.Entity("EmissionsMonitorModel.VirtualFileSystem.Folder", b =>
                {
                    b.Property<int>("FolderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FolderId"), 1L, 1);

                    b.Property<string>("FolderName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ParentFolderId")
                        .HasColumnType("int");

                    b.HasKey("FolderId");

                    b.HasIndex("ParentFolderId");

                    b.ToTable("FOLDER", (string)null);
                });

            modelBuilder.Entity("EmissionsMonitorModel.VirtualFileSystem.SaveItem", b =>
                {
                    b.Property<int>("SaveItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SaveItemId"), 1L, 1);

                    b.Property<int>("FolderId")
                        .HasColumnType("int");

                    b.Property<string>("SaveItemName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SaveItemType")
                        .HasColumnType("int");

                    b.HasKey("SaveItemId");

                    b.HasIndex("FolderId");

                    b.ToTable("SAVE_ITEM", (string)null);

                    b.HasDiscriminator<int>("SaveItemType").HasValue(0);
                });

            modelBuilder.Entity("EmissionsMonitorModel.VirtualFileSystem.ExploreSetSaveItem", b =>
                {
                    b.HasBaseType("EmissionsMonitorModel.VirtualFileSystem.SaveItem");

                    b.Property<string>("ExploreSetJsonDetails")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue(1);
                });

            modelBuilder.Entity("EmissionsMonitorModel.VirtualFileSystem.ModelSaveItem", b =>
                {
                    b.HasBaseType("EmissionsMonitorModel.VirtualFileSystem.SaveItem");

                    b.Property<string>("ProcessModelJsonDetails")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue(2);
                });

            modelBuilder.Entity("EmissionsMonitorModel.VirtualFileSystem.Folder", b =>
                {
                    b.HasOne("EmissionsMonitorModel.VirtualFileSystem.Folder", null)
                        .WithMany("ChildFolders")
                        .HasForeignKey("ParentFolderId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("EmissionsMonitorModel.VirtualFileSystem.SaveItem", b =>
                {
                    b.HasOne("EmissionsMonitorModel.VirtualFileSystem.Folder", "Folder")
                        .WithMany("SaveItems")
                        .HasForeignKey("FolderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Folder");
                });

            modelBuilder.Entity("EmissionsMonitorModel.VirtualFileSystem.Folder", b =>
                {
                    b.Navigation("ChildFolders");

                    b.Navigation("SaveItems");
                });
#pragma warning restore 612, 618
        }
    }
}
