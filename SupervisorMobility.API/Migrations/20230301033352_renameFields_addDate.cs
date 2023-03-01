using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class renameFieldsaddDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nomina",
                table: "Users",
                newName: "Payroll");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Users",
                newName: "Name");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "Date",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DisabledDate",
                table: "Users",
                type: "Date",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "Users",
                type: "Date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "CreatedDate", "DisabledDate", "LastUpdated" },
                values: new object[] { null, null, new DateTime(2023, 2, 28, 21, 33, 52, 44, DateTimeKind.Local).AddTicks(9106) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DisabledDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Payroll",
                table: "Users",
                newName: "Nomina");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Users",
                newName: "Nombre");
        }
    }
}
