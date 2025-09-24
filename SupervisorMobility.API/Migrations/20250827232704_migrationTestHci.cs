using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class migrationTestHci : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SOSHubId",
                table: "HCIs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HCIs_SOSHubId",
                table: "HCIs",
                column: "SOSHubId");

            migrationBuilder.AddForeignKey(
                name: "FK_HCIs_SOSHubs_SOSHubId",
                table: "HCIs",
                column: "SOSHubId",
                principalTable: "SOSHubs",
                principalColumn: "SOSHubId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 17, 27, 3, 431, DateTimeKind.Local).AddTicks(1937));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 3,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 17, 27, 3, 431, DateTimeKind.Local).AddTicks(1939));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 4,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 17, 27, 3, 431, DateTimeKind.Local).AddTicks(1940));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 5,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 17, 27, 3, 431, DateTimeKind.Local).AddTicks(1942));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 6,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 17, 27, 3, 431, DateTimeKind.Local).AddTicks(1944));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 7,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 17, 27, 3, 431, DateTimeKind.Local).AddTicks(1945));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 8,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 17, 27, 3, 431, DateTimeKind.Local).AddTicks(1947));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 9,
                column: "EntryDate",
                value: new DateTime(2025, 8, 27, 17, 27, 3, 431, DateTimeKind.Local).AddTicks(1949));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 17, 27, 3, 431, DateTimeKind.Local).AddTicks(1708));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 17, 27, 3, 431, DateTimeKind.Local).AddTicks(1725));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 17, 27, 3, 431, DateTimeKind.Local).AddTicks(1727));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 17, 27, 3, 431, DateTimeKind.Local).AddTicks(1729));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 17, 27, 3, 431, DateTimeKind.Local).AddTicks(1732));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 17, 27, 3, 431, DateTimeKind.Local).AddTicks(1735));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 17, 27, 3, 431, DateTimeKind.Local).AddTicks(1737));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2025, 8, 27, 17, 27, 3, 431, DateTimeKind.Local).AddTicks(1740));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HCIs_SOSHubs_SOSHubId",
                table: "HCIs");

            migrationBuilder.DropIndex(
                name: "IX_HCIs_SOSHubId",
                table: "HCIs");

            migrationBuilder.DropColumn(
                name: "SOSHubId",
                table: "HCIs");

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
    }
}
