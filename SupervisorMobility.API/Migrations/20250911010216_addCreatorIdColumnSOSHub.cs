using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class addCreatorIdColumnSOSHub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "SOSHubsHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "SOSHubs",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "EntryDate",
                value: new DateTime(2025, 9, 10, 19, 2, 15, 117, DateTimeKind.Local).AddTicks(5413));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 3,
                column: "EntryDate",
                value: new DateTime(2025, 9, 10, 19, 2, 15, 117, DateTimeKind.Local).AddTicks(5415));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 4,
                column: "EntryDate",
                value: new DateTime(2025, 9, 10, 19, 2, 15, 117, DateTimeKind.Local).AddTicks(5417));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 5,
                column: "EntryDate",
                value: new DateTime(2025, 9, 10, 19, 2, 15, 117, DateTimeKind.Local).AddTicks(5419));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 6,
                column: "EntryDate",
                value: new DateTime(2025, 9, 10, 19, 2, 15, 117, DateTimeKind.Local).AddTicks(5420));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 7,
                column: "EntryDate",
                value: new DateTime(2025, 9, 10, 19, 2, 15, 117, DateTimeKind.Local).AddTicks(5422));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 8,
                column: "EntryDate",
                value: new DateTime(2025, 9, 10, 19, 2, 15, 117, DateTimeKind.Local).AddTicks(5425));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 9,
                column: "EntryDate",
                value: new DateTime(2025, 9, 10, 19, 2, 15, 117, DateTimeKind.Local).AddTicks(5426));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 19, 2, 15, 117, DateTimeKind.Local).AddTicks(5179));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 19, 2, 15, 117, DateTimeKind.Local).AddTicks(5208));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 19, 2, 15, 117, DateTimeKind.Local).AddTicks(5210));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 19, 2, 15, 117, DateTimeKind.Local).AddTicks(5212));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 19, 2, 15, 117, DateTimeKind.Local).AddTicks(5215));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 19, 2, 15, 117, DateTimeKind.Local).AddTicks(5218));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 19, 2, 15, 117, DateTimeKind.Local).AddTicks(5220));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 19, 2, 15, 117, DateTimeKind.Local).AddTicks(5222));

            migrationBuilder.CreateIndex(
                name: "IX_SOSHubsHistory_CreatorId",
                table: "SOSHubsHistory",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSHubs_CreatorId",
                table: "SOSHubs",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_SOSHubs_Users_CreatorId",
                table: "SOSHubs",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SOSHubsHistory_Users_CreatorId",
                table: "SOSHubsHistory",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SOSHubs_Users_CreatorId",
                table: "SOSHubs");

            migrationBuilder.DropForeignKey(
                name: "FK_SOSHubsHistory_Users_CreatorId",
                table: "SOSHubsHistory");

            migrationBuilder.DropIndex(
                name: "IX_SOSHubsHistory_CreatorId",
                table: "SOSHubsHistory");

            migrationBuilder.DropIndex(
                name: "IX_SOSHubs_CreatorId",
                table: "SOSHubs");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "SOSHubsHistory");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "SOSHubs");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "EntryDate",
                value: new DateTime(2025, 9, 8, 20, 31, 47, 435, DateTimeKind.Local).AddTicks(6295));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 3,
                column: "EntryDate",
                value: new DateTime(2025, 9, 8, 20, 31, 47, 435, DateTimeKind.Local).AddTicks(6297));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 4,
                column: "EntryDate",
                value: new DateTime(2025, 9, 8, 20, 31, 47, 435, DateTimeKind.Local).AddTicks(6299));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 5,
                column: "EntryDate",
                value: new DateTime(2025, 9, 8, 20, 31, 47, 435, DateTimeKind.Local).AddTicks(6300));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 6,
                column: "EntryDate",
                value: new DateTime(2025, 9, 8, 20, 31, 47, 435, DateTimeKind.Local).AddTicks(6302));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 7,
                column: "EntryDate",
                value: new DateTime(2025, 9, 8, 20, 31, 47, 435, DateTimeKind.Local).AddTicks(6304));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 8,
                column: "EntryDate",
                value: new DateTime(2025, 9, 8, 20, 31, 47, 435, DateTimeKind.Local).AddTicks(6306));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 9,
                column: "EntryDate",
                value: new DateTime(2025, 9, 8, 20, 31, 47, 435, DateTimeKind.Local).AddTicks(6307));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 8, 20, 31, 47, 435, DateTimeKind.Local).AddTicks(6071));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 8, 20, 31, 47, 435, DateTimeKind.Local).AddTicks(6092));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 8, 20, 31, 47, 435, DateTimeKind.Local).AddTicks(6095));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 8, 20, 31, 47, 435, DateTimeKind.Local).AddTicks(6096));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 8, 20, 31, 47, 435, DateTimeKind.Local).AddTicks(6100));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 8, 20, 31, 47, 435, DateTimeKind.Local).AddTicks(6111));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 8, 20, 31, 47, 435, DateTimeKind.Local).AddTicks(6123));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 8, 20, 31, 47, 435, DateTimeKind.Local).AddTicks(6126));
        }
    }
}
