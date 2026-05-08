using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class AddYearColumnAtDailyAndWeeklyRevision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "WeeklyRevisions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "RevisionDate",
                table: "DailyRevisions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "DailyRevisions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Year",
                table: "WeeklyRevisions");

            migrationBuilder.DropColumn(
                name: "RevisionDate",
                table: "DailyRevisions");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "DailyRevisions");
        }
    }
}
