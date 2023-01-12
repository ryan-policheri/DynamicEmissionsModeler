using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmissionsMonitorDataAccess.Database.Migrations
{
    public partial class nodeinspectexperiement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NODE_INSPECT_EXPERIMENT",
                columns: table => new
                {
                    ExperimentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModelId = table.Column<int>(type: "int", nullable: false),
                    NodeId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DataResolution = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExperimentDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NODE_INSPECT_EXPERIMENT", x => x.ExperimentId);
                });

            migrationBuilder.CreateTable(
                name: "NODE_INSPECT_EXPERIMENT_RECORD",
                columns: table => new
                {
                    ExperimentId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ProductTotalInDefaultUnit = table.Column<double>(type: "float", nullable: false),
                    ProductDefaultUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CO2EmissionsInKilograms = table.Column<double>(type: "float", nullable: false),
                    FuelCostInDollars = table.Column<double>(type: "float", nullable: false),
                    CO2EmissionsPerDefaultProductUnit = table.Column<double>(type: "float", nullable: false),
                    FuelCostPerDefaultProductUnit = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NODE_INSPECT_EXPERIMENT_RECORD", x => new { x.ExperimentId, x.Timestamp });
                    table.ForeignKey(
                        name: "FK_NODE_INSPECT_EXPERIMENT_RECORD_NODE_INSPECT_EXPERIMENT_ExperimentId",
                        column: x => x.ExperimentId,
                        principalTable: "NODE_INSPECT_EXPERIMENT",
                        principalColumn: "ExperimentId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NODE_INSPECT_EXPERIMENT_RECORD");

            migrationBuilder.DropTable(
                name: "NODE_INSPECT_EXPERIMENT");
        }
    }
}
