using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class removeIsMachineOperationColumnSection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMachineOperation",
                table: "Sections");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "EntryDate",
                value: new DateTime(2025, 9, 24, 17, 33, 9, 939, DateTimeKind.Local).AddTicks(4050));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 3,
                column: "EntryDate",
                value: new DateTime(2025, 9, 24, 17, 33, 9, 939, DateTimeKind.Local).AddTicks(4053));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 4,
                column: "EntryDate",
                value: new DateTime(2025, 9, 24, 17, 33, 9, 939, DateTimeKind.Local).AddTicks(4055));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 5,
                column: "EntryDate",
                value: new DateTime(2025, 9, 24, 17, 33, 9, 939, DateTimeKind.Local).AddTicks(4057));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 6,
                column: "EntryDate",
                value: new DateTime(2025, 9, 24, 17, 33, 9, 939, DateTimeKind.Local).AddTicks(4059));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 7,
                column: "EntryDate",
                value: new DateTime(2025, 9, 24, 17, 33, 9, 939, DateTimeKind.Local).AddTicks(4061));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 8,
                column: "EntryDate",
                value: new DateTime(2025, 9, 24, 17, 33, 9, 939, DateTimeKind.Local).AddTicks(4063));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 9,
                column: "EntryDate",
                value: new DateTime(2025, 9, 24, 17, 33, 9, 939, DateTimeKind.Local).AddTicks(4064));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 24, 17, 33, 9, 939, DateTimeKind.Local).AddTicks(3784));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 24, 17, 33, 9, 939, DateTimeKind.Local).AddTicks(3812));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 24, 17, 33, 9, 939, DateTimeKind.Local).AddTicks(3814));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 24, 17, 33, 9, 939, DateTimeKind.Local).AddTicks(3816));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 24, 17, 33, 9, 939, DateTimeKind.Local).AddTicks(3819));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 24, 17, 33, 9, 939, DateTimeKind.Local).AddTicks(3822));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 24, 17, 33, 9, 939, DateTimeKind.Local).AddTicks(3829));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 24, 17, 33, 9, 939, DateTimeKind.Local).AddTicks(3832));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                value: new DateTime(2025, 9, 10, 20, 40, 53, 305, DateTimeKind.Local).AddTicks(1988));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 3,
                column: "EntryDate",
                value: new DateTime(2025, 9, 10, 20, 40, 53, 305, DateTimeKind.Local).AddTicks(1990));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 4,
                column: "EntryDate",
                value: new DateTime(2025, 9, 10, 20, 40, 53, 305, DateTimeKind.Local).AddTicks(1992));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 5,
                column: "EntryDate",
                value: new DateTime(2025, 9, 10, 20, 40, 53, 305, DateTimeKind.Local).AddTicks(1994));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 6,
                column: "EntryDate",
                value: new DateTime(2025, 9, 10, 20, 40, 53, 305, DateTimeKind.Local).AddTicks(1995));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 7,
                column: "EntryDate",
                value: new DateTime(2025, 9, 10, 20, 40, 53, 305, DateTimeKind.Local).AddTicks(1997));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 8,
                column: "EntryDate",
                value: new DateTime(2025, 9, 10, 20, 40, 53, 305, DateTimeKind.Local).AddTicks(2002));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 9,
                column: "EntryDate",
                value: new DateTime(2025, 9, 10, 20, 40, 53, 305, DateTimeKind.Local).AddTicks(2004));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 20, 40, 53, 305, DateTimeKind.Local).AddTicks(1743));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 20, 40, 53, 305, DateTimeKind.Local).AddTicks(1759));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 20, 40, 53, 305, DateTimeKind.Local).AddTicks(1761));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 20, 40, 53, 305, DateTimeKind.Local).AddTicks(1766));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 20, 40, 53, 305, DateTimeKind.Local).AddTicks(1770));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 20, 40, 53, 305, DateTimeKind.Local).AddTicks(1772));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 20, 40, 53, 305, DateTimeKind.Local).AddTicks(1775));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 20, 40, 53, 305, DateTimeKind.Local).AddTicks(1777));
        }
    }
}
