using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class addIsMachineOperationToSections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMachineOperation",
                table: "Sections",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "EntryDate",
                value: new DateTime(2025, 8, 28, 12, 34, 13, 705, DateTimeKind.Local).AddTicks(9311));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 3,
                column: "EntryDate",
                value: new DateTime(2025, 8, 28, 12, 34, 13, 705, DateTimeKind.Local).AddTicks(9313));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 4,
                column: "EntryDate",
                value: new DateTime(2025, 8, 28, 12, 34, 13, 705, DateTimeKind.Local).AddTicks(9315));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 5,
                column: "EntryDate",
                value: new DateTime(2025, 8, 28, 12, 34, 13, 705, DateTimeKind.Local).AddTicks(9317));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 6,
                column: "EntryDate",
                value: new DateTime(2025, 8, 28, 12, 34, 13, 705, DateTimeKind.Local).AddTicks(9319));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 7,
                column: "EntryDate",
                value: new DateTime(2025, 8, 28, 12, 34, 13, 705, DateTimeKind.Local).AddTicks(9321));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 8,
                column: "EntryDate",
                value: new DateTime(2025, 8, 28, 12, 34, 13, 705, DateTimeKind.Local).AddTicks(9323));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 9,
                column: "EntryDate",
                value: new DateTime(2025, 8, 28, 12, 34, 13, 705, DateTimeKind.Local).AddTicks(9324));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 28, 12, 34, 13, 705, DateTimeKind.Local).AddTicks(9084));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 28, 12, 34, 13, 705, DateTimeKind.Local).AddTicks(9099));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 28, 12, 34, 13, 705, DateTimeKind.Local).AddTicks(9101));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 28, 12, 34, 13, 705, DateTimeKind.Local).AddTicks(9110));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 28, 12, 34, 13, 705, DateTimeKind.Local).AddTicks(9121));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 28, 12, 34, 13, 705, DateTimeKind.Local).AddTicks(9124));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 28, 12, 34, 13, 705, DateTimeKind.Local).AddTicks(9126));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 28, 12, 34, 13, 705, DateTimeKind.Local).AddTicks(9128));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMachineOperation",
                table: "Sections");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 18, 44, 36, 121, DateTimeKind.Local).AddTicks(1637));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 3,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 18, 44, 36, 121, DateTimeKind.Local).AddTicks(1640));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 4,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 18, 44, 36, 121, DateTimeKind.Local).AddTicks(1642));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 5,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 18, 44, 36, 121, DateTimeKind.Local).AddTicks(1644));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 6,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 18, 44, 36, 121, DateTimeKind.Local).AddTicks(1645));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 7,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 18, 44, 36, 121, DateTimeKind.Local).AddTicks(1647));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 8,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 18, 44, 36, 121, DateTimeKind.Local).AddTicks(1654));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 9,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 18, 44, 36, 121, DateTimeKind.Local).AddTicks(1656));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 18, 44, 36, 121, DateTimeKind.Local).AddTicks(1435));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 18, 44, 36, 121, DateTimeKind.Local).AddTicks(1453));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 18, 44, 36, 121, DateTimeKind.Local).AddTicks(1455));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 18, 44, 36, 121, DateTimeKind.Local).AddTicks(1457));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 18, 44, 36, 121, DateTimeKind.Local).AddTicks(1461));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 18, 44, 36, 121, DateTimeKind.Local).AddTicks(1464));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 18, 44, 36, 121, DateTimeKind.Local).AddTicks(1475));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 18, 44, 36, 121, DateTimeKind.Local).AddTicks(1484));
        }
    }
}
