using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmissionsMonitorDataAccess.Database.Migrations
{
    public partial class Modelsaveitem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProcessModelJsonDetails",
                table: "SAVE_ITEM",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessModelJsonDetails",
                table: "SAVE_ITEM");
        }
    }
}
