using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmissionsMonitorDataAccess.Database.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DATA_SOURCE",
                columns: table => new
                {
                    SourceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceType = table.Column<int>(type: "int", nullable: false),
                    SourceDetailsJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DATA_SOURCE", x => x.SourceId);
                });

            migrationBuilder.CreateTable(
                name: "FILE_SYSTEM",
                columns: table => new
                {
                    FileSystemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileSystemName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FILE_SYSTEM", x => x.FileSystemId);
                });

            migrationBuilder.CreateTable(
                name: "FOLDER",
                columns: table => new
                {
                    FolderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwningFileSystemId = table.Column<int>(type: "int", nullable: false),
                    ParentFolderId = table.Column<int>(type: "int", nullable: false),
                    FolderName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FOLDER", x => x.FolderId);
                    table.ForeignKey(
                        name: "FK_FOLDER_FILE_SYSTEM_OwningFileSystemId",
                        column: x => x.OwningFileSystemId,
                        principalTable: "FILE_SYSTEM",
                        principalColumn: "FileSystemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FOLDER_FOLDER_ParentFolderId",
                        column: x => x.ParentFolderId,
                        principalTable: "FOLDER",
                        principalColumn: "FolderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaveItems",
                columns: table => new
                {
                    SaveItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileSystemId = table.Column<int>(type: "int", nullable: false),
                    FolderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaveItems", x => x.SaveItemId);
                    table.ForeignKey(
                        name: "FK_SaveItems_FILE_SYSTEM_FileSystemId",
                        column: x => x.FileSystemId,
                        principalTable: "FILE_SYSTEM",
                        principalColumn: "FileSystemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaveItems_FOLDER_FolderId",
                        column: x => x.FolderId,
                        principalTable: "FOLDER",
                        principalColumn: "FolderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FOLDER_OwningFileSystemId",
                table: "FOLDER",
                column: "OwningFileSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_FOLDER_ParentFolderId",
                table: "FOLDER",
                column: "ParentFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_SaveItems_FileSystemId",
                table: "SaveItems",
                column: "FileSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_SaveItems_FolderId",
                table: "SaveItems",
                column: "FolderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DATA_SOURCE");

            migrationBuilder.DropTable(
                name: "SaveItems");

            migrationBuilder.DropTable(
                name: "FOLDER");

            migrationBuilder.DropTable(
                name: "FILE_SYSTEM");
        }
    }
}
