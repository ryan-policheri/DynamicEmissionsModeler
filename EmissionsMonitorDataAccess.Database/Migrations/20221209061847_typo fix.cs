using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmissionsMonitorDataAccess.Database.Migrations
{
    public partial class typofix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExperimentData",
                table: "IND_STUDY_EXPERIMENT",
                newName: "ExperimentDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExperimentDate",
                table: "IND_STUDY_EXPERIMENT",
                newName: "ExperimentData");
        }
    }
}
