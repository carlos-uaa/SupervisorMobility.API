using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class NewJobObs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "dateStart",
                table: "JobObservations",
                newName: "DateStart");

            migrationBuilder.RenameColumn(
                name: "dateEnd",
                table: "JobObservations",
                newName: "DateEnd");

            migrationBuilder.AddColumn<string>(
                name: "Justification",
                table: "JobObservations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlannedEndDate",
                table: "JobObservations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlannedStartDate",
                table: "JobObservations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "JobObservations",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AssyCharts",
                keyColumn: "AssyChardId",
                keyValue: 1,
                columns: new[] { "CCP", "GOS", "HOE" },
                values: new object[] { "TX2300-5NA_1", "CSV TX2300-5NA_1", "TX2300-5NA_1" });

            migrationBuilder.UpdateData(
                table: "JobObservations",
                keyColumn: "JobObservationId",
                keyValue: 1,
                columns: new[] { "Justification", "PlannedEndDate", "PlannedStartDate", "Status" },
                values: new object[] { null, null, null, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Justification",
                table: "JobObservations");

            migrationBuilder.DropColumn(
                name: "PlannedEndDate",
                table: "JobObservations");

            migrationBuilder.DropColumn(
                name: "PlannedStartDate",
                table: "JobObservations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "JobObservations");

            migrationBuilder.RenameColumn(
                name: "DateStart",
                table: "JobObservations",
                newName: "dateStart");

            migrationBuilder.RenameColumn(
                name: "DateEnd",
                table: "JobObservations",
                newName: "dateEnd");

            migrationBuilder.UpdateData(
                table: "AssyCharts",
                keyColumn: "AssyChardId",
                keyValue: 1,
                columns: new[] { "CCP", "GOS", "HOE" },
                values: new object[] { "string", "string", "string" });
        }
    }
}
