using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmissionsMonitorDataAccess.Database.Migrations
{
    public partial class dailycarbonexperiement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DAILY_CARBON_EXPERIMENT",
                columns: table => new
                {
                    ExperimentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModelId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExperimentDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    NodeIdsString = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DAILY_CARBON_EXPERIMENT", x => x.ExperimentId);
                });

            migrationBuilder.CreateTable(
                name: "DAILY_CARBON_EXPERIMENT_RECORD",
                columns: table => new
                {
                    ExperimentId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Co2InKilograms = table.Column<double>(type: "float", nullable: false),
                    Co2InMegatons = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DAILY_CARBON_EXPERIMENT_RECORD", x => new { x.ExperimentId, x.Date });
                    table.ForeignKey(
                        name: "FK_DAILY_CARBON_EXPERIMENT_RECORD_DAILY_CARBON_EXPERIMENT_ExperimentId",
                        column: x => x.ExperimentId,
                        principalTable: "DAILY_CARBON_EXPERIMENT",
                        principalColumn: "ExperimentId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DAILY_CARBON_EXPERIMENT_RECORD");

            migrationBuilder.DropTable(
                name: "DAILY_CARBON_EXPERIMENT");
        }
    }
}
