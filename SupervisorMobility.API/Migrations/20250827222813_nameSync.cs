using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class nameSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 28, 7, 989, DateTimeKind.Local).AddTicks(9993));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 3,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 28, 7, 989, DateTimeKind.Local).AddTicks(9995));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 4,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 28, 7, 989, DateTimeKind.Local).AddTicks(9997));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 5,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 28, 7, 989, DateTimeKind.Local).AddTicks(9998));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 6,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 28, 7, 990, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 7,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 28, 7, 990, DateTimeKind.Local).AddTicks(2));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 8,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 28, 7, 990, DateTimeKind.Local).AddTicks(3));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 9,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 28, 7, 990, DateTimeKind.Local).AddTicks(5));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 28, 7, 989, DateTimeKind.Local).AddTicks(9844));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 28, 7, 989, DateTimeKind.Local).AddTicks(9852));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 28, 7, 989, DateTimeKind.Local).AddTicks(9854));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 28, 7, 989, DateTimeKind.Local).AddTicks(9855));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 28, 7, 989, DateTimeKind.Local).AddTicks(9859));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 28, 7, 989, DateTimeKind.Local).AddTicks(9861));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 28, 7, 989, DateTimeKind.Local).AddTicks(9863));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 28, 7, 989, DateTimeKind.Local).AddTicks(9866));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
