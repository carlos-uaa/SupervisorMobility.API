using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 6, 22, 533, DateTimeKind.Local).AddTicks(8181));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 3,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 6, 22, 533, DateTimeKind.Local).AddTicks(8183));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 4,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 6, 22, 533, DateTimeKind.Local).AddTicks(8184));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 5,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 6, 22, 533, DateTimeKind.Local).AddTicks(8187));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 6,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 6, 22, 533, DateTimeKind.Local).AddTicks(8189));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 7,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 6, 22, 533, DateTimeKind.Local).AddTicks(8190));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 8,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 6, 22, 533, DateTimeKind.Local).AddTicks(8192));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 9,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 6, 22, 533, DateTimeKind.Local).AddTicks(8193));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 6, 22, 533, DateTimeKind.Local).AddTicks(8038));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 6, 22, 533, DateTimeKind.Local).AddTicks(8046));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 6, 22, 533, DateTimeKind.Local).AddTicks(8048));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 6, 22, 533, DateTimeKind.Local).AddTicks(8049));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 6, 22, 533, DateTimeKind.Local).AddTicks(8056));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 6, 22, 533, DateTimeKind.Local).AddTicks(8059));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 6, 22, 533, DateTimeKind.Local).AddTicks(8061));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 6, 22, 533, DateTimeKind.Local).AddTicks(8063));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 0, 0, 149, DateTimeKind.Local).AddTicks(1360));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 3,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 0, 0, 149, DateTimeKind.Local).AddTicks(1362));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 4,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 0, 0, 149, DateTimeKind.Local).AddTicks(1364));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 5,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 0, 0, 149, DateTimeKind.Local).AddTicks(1366));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 6,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 0, 0, 149, DateTimeKind.Local).AddTicks(1367));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 7,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 0, 0, 149, DateTimeKind.Local).AddTicks(1369));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 8,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 0, 0, 149, DateTimeKind.Local).AddTicks(1371));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 9,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 0, 0, 149, DateTimeKind.Local).AddTicks(1373));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 0, 0, 149, DateTimeKind.Local).AddTicks(1151));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 0, 0, 149, DateTimeKind.Local).AddTicks(1164));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 0, 0, 149, DateTimeKind.Local).AddTicks(1166));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 0, 0, 149, DateTimeKind.Local).AddTicks(1168));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 0, 0, 149, DateTimeKind.Local).AddTicks(1174));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 0, 0, 149, DateTimeKind.Local).AddTicks(1176));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 0, 0, 149, DateTimeKind.Local).AddTicks(1179));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 0, 0, 149, DateTimeKind.Local).AddTicks(1188));
        }
    }
}
