using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HourmeterId",
                table: "DailyRevisions");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "WeeklyRevisions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "RevisionCycles",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "HRIRevisionItems",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "HRICycles",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "HourmeterRevisions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "DailyRevisions",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "WeeklyRevisions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "RevisionCycles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "HRIRevisionItems");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "HRICycles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "HourmeterRevisions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "DailyRevisions");

            migrationBuilder.AddColumn<int>(
                name: "HourmeterId",
                table: "DailyRevisions",
                type: "int",
                nullable: true);
        }
    }
}
