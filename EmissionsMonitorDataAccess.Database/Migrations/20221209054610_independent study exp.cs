using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmissionsMonitorDataAccess.Database.Migrations
{
    public partial class independentstudyexp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Co2InMegatons",
                table: "DAILY_CARBON_EXPERIMENT_RECORD",
                newName: "Co2InMetricTons");

            migrationBuilder.CreateTable(
                name: "IND_STUDY_EXPERIMENT",
                columns: table => new
                {
                    ExperimentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModelId = table.Column<int>(type: "int", nullable: false),
                    FinalSteamNodeId = table.Column<int>(type: "int", nullable: false),
                    FinalElectricNodeId = table.Column<int>(type: "int", nullable: false),
                    FinalChilledWaterNodeId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DataResolution = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExperimentData = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IND_STUDY_EXPERIMENT", x => x.ExperimentId);
                });

            migrationBuilder.CreateTable(
                name: "IND_STUDY_EXPERIMENT_RECORD",
                columns: table => new
                {
                    ExperimentId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TotalEmissionsInMetricTons = table.Column<double>(type: "float", nullable: false),
                    TotalCostInDollars = table.Column<double>(type: "float", nullable: false),
                    SteamTotalInMMBTU = table.Column<double>(type: "float", nullable: false),
                    SteamEmissionsInMetricTons = table.Column<double>(type: "float", nullable: false),
                    SteamCostInDollars = table.Column<double>(type: "float", nullable: false),
                    SteamEmissionsFactorInKilogramsCo2PerMMBTU = table.Column<double>(type: "float", nullable: false),
                    SteamCostDollarsPerMMBTU = table.Column<double>(type: "float", nullable: false),
                    ElectricTotalInKwh = table.Column<double>(type: "float", nullable: false),
                    ElectricEmissionsInMetricTons = table.Column<double>(type: "float", nullable: false),
                    ElectricCostInDollars = table.Column<double>(type: "float", nullable: false),
                    ElectricEmissionsFactorInKilogramsCo2PerKwh = table.Column<double>(type: "float", nullable: false),
                    ElectricCostPerKwh = table.Column<double>(type: "float", nullable: false),
                    ChilledWaterTotalInGallons = table.Column<double>(type: "float", nullable: false),
                    ChilledWaterEmissionsInMetricTons = table.Column<double>(type: "float", nullable: false),
                    ChilledWaterCostInDollars = table.Column<double>(type: "float", nullable: false),
                    ChilledWaterEmissionsFactorInKilogramsCo2PerGallon = table.Column<double>(type: "float", nullable: false),
                    ChilledWaterCostPerGallon = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IND_STUDY_EXPERIMENT_RECORD", x => new { x.ExperimentId, x.Timestamp });
                    table.ForeignKey(
                        name: "FK_IND_STUDY_EXPERIMENT_RECORD_IND_STUDY_EXPERIMENT_ExperimentId",
                        column: x => x.ExperimentId,
                        principalTable: "IND_STUDY_EXPERIMENT",
                        principalColumn: "ExperimentId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IND_STUDY_EXPERIMENT_RECORD");

            migrationBuilder.DropTable(
                name: "IND_STUDY_EXPERIMENT");

            migrationBuilder.RenameColumn(
                name: "Co2InMetricTons",
                table: "DAILY_CARBON_EXPERIMENT_RECORD",
                newName: "Co2InMegatons");
        }
    }
}
