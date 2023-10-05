using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmissionsMonitorDataAccess.Database.Migrations
{
    public partial class Renameschema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "EmissionsMonitor");

            migrationBuilder.RenameTable(
                name: "SAVE_ITEM",
                newName: "SAVE_ITEM",
                newSchema: "EmissionsMonitor");

            migrationBuilder.RenameTable(
                name: "NODE_INSPECT_EXPERIMENT_RECORD",
                newName: "NODE_INSPECT_EXPERIMENT_RECORD",
                newSchema: "EmissionsMonitor");

            migrationBuilder.RenameTable(
                name: "NODE_INSPECT_EXPERIMENT",
                newName: "NODE_INSPECT_EXPERIMENT",
                newSchema: "EmissionsMonitor");

            migrationBuilder.RenameTable(
                name: "IND_STUDY_EXPERIMENT_RECORD",
                newName: "IND_STUDY_EXPERIMENT_RECORD",
                newSchema: "EmissionsMonitor");

            migrationBuilder.RenameTable(
                name: "IND_STUDY_EXPERIMENT",
                newName: "IND_STUDY_EXPERIMENT",
                newSchema: "EmissionsMonitor");

            migrationBuilder.RenameTable(
                name: "FOLDER",
                newName: "FOLDER",
                newSchema: "EmissionsMonitor");

            migrationBuilder.RenameTable(
                name: "DATA_SOURCE",
                newName: "DATA_SOURCE",
                newSchema: "EmissionsMonitor");

            migrationBuilder.RenameTable(
                name: "DAILY_CARBON_EXPERIMENT_RECORD",
                newName: "DAILY_CARBON_EXPERIMENT_RECORD",
                newSchema: "EmissionsMonitor");

            migrationBuilder.RenameTable(
                name: "DAILY_CARBON_EXPERIMENT",
                newName: "DAILY_CARBON_EXPERIMENT",
                newSchema: "EmissionsMonitor");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "SAVE_ITEM",
                schema: "EmissionsMonitor",
                newName: "SAVE_ITEM");

            migrationBuilder.RenameTable(
                name: "NODE_INSPECT_EXPERIMENT_RECORD",
                schema: "EmissionsMonitor",
                newName: "NODE_INSPECT_EXPERIMENT_RECORD");

            migrationBuilder.RenameTable(
                name: "NODE_INSPECT_EXPERIMENT",
                schema: "EmissionsMonitor",
                newName: "NODE_INSPECT_EXPERIMENT");

            migrationBuilder.RenameTable(
                name: "IND_STUDY_EXPERIMENT_RECORD",
                schema: "EmissionsMonitor",
                newName: "IND_STUDY_EXPERIMENT_RECORD");

            migrationBuilder.RenameTable(
                name: "IND_STUDY_EXPERIMENT",
                schema: "EmissionsMonitor",
                newName: "IND_STUDY_EXPERIMENT");

            migrationBuilder.RenameTable(
                name: "FOLDER",
                schema: "EmissionsMonitor",
                newName: "FOLDER");

            migrationBuilder.RenameTable(
                name: "DATA_SOURCE",
                schema: "EmissionsMonitor",
                newName: "DATA_SOURCE");

            migrationBuilder.RenameTable(
                name: "DAILY_CARBON_EXPERIMENT_RECORD",
                schema: "EmissionsMonitor",
                newName: "DAILY_CARBON_EXPERIMENT_RECORD");

            migrationBuilder.RenameTable(
                name: "DAILY_CARBON_EXPERIMENT",
                schema: "EmissionsMonitor",
                newName: "DAILY_CARBON_EXPERIMENT");
        }
    }
}
