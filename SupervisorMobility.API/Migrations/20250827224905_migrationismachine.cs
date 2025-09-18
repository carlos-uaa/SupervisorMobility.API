using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class migrationismachine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMachineOperation",
                table: "Analyses");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 49, 4, 183, DateTimeKind.Local).AddTicks(7865));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 3,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 49, 4, 183, DateTimeKind.Local).AddTicks(7867));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 4,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 49, 4, 183, DateTimeKind.Local).AddTicks(7869));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 5,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 49, 4, 183, DateTimeKind.Local).AddTicks(7871));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 6,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 49, 4, 183, DateTimeKind.Local).AddTicks(7873));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 7,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 49, 4, 183, DateTimeKind.Local).AddTicks(7874));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 8,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 49, 4, 183, DateTimeKind.Local).AddTicks(7876));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 9,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 16, 49, 4, 183, DateTimeKind.Local).AddTicks(7878));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 49, 4, 183, DateTimeKind.Local).AddTicks(7687));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 49, 4, 183, DateTimeKind.Local).AddTicks(7693));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 49, 4, 183, DateTimeKind.Local).AddTicks(7695));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 49, 4, 183, DateTimeKind.Local).AddTicks(7696));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 49, 4, 183, DateTimeKind.Local).AddTicks(7700));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 49, 4, 183, DateTimeKind.Local).AddTicks(7703));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 49, 4, 183, DateTimeKind.Local).AddTicks(7705));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 16, 49, 4, 183, DateTimeKind.Local).AddTicks(7707));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMachineOperation",
                table: "Analyses",
                type: "bit",
                nullable: false,
                defaultValue: false);

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
    }
}
