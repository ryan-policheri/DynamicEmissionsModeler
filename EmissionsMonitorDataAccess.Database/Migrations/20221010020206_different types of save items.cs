using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmissionsMonitorDataAccess.Database.Migrations
{
    public partial class differenttypesofsaveitems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExploreSetJsonDetails",
                table: "SAVE_ITEM",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaveItemType",
                table: "SAVE_ITEM",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExploreSetJsonDetails",
                table: "SAVE_ITEM");

            migrationBuilder.DropColumn(
                name: "SaveItemType",
                table: "SAVE_ITEM");
        }
    }
}
