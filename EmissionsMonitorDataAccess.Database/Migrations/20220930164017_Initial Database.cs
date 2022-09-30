using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmissionsMonitorDataAccess.Database.Migrations
{
    public partial class InitialDatabase : Migration
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
                name: "FOLDER",
                columns: table => new
                {
                    FolderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentFolderId = table.Column<int>(type: "int", nullable: true),
                    FolderName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FOLDER", x => x.FolderId);
                    table.ForeignKey(
                        name: "FK_FOLDER_FOLDER_ParentFolderId",
                        column: x => x.ParentFolderId,
                        principalTable: "FOLDER",
                        principalColumn: "FolderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SAVE_ITEM",
                columns: table => new
                {
                    SaveItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FolderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SAVE_ITEM", x => x.SaveItemId);
                    table.ForeignKey(
                        name: "FK_SAVE_ITEM_FOLDER_FolderId",
                        column: x => x.FolderId,
                        principalTable: "FOLDER",
                        principalColumn: "FolderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FOLDER_ParentFolderId",
                table: "FOLDER",
                column: "ParentFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_SAVE_ITEM_FolderId",
                table: "SAVE_ITEM",
                column: "FolderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DATA_SOURCE");

            migrationBuilder.DropTable(
                name: "SAVE_ITEM");

            migrationBuilder.DropTable(
                name: "FOLDER");
        }
    }
}
