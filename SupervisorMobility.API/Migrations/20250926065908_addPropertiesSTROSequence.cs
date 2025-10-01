using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class addPropertiesSTROSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SOSSynopticRequirementsOperationSequences_SOSSynopticTableofOperatingRequirements_SOSSynopticTableofOperatingRequirementsId",
                table: "SOSSynopticRequirementsOperationSequences");

            migrationBuilder.AlterColumn<int>(
                name: "SOSSynopticTableofOperatingRequirementsId",
                table: "SOSSynopticRequirementsOperationSequences",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOperationMachineRequired",
                table: "SOSSynopticRequirementsOperationSequences",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOperationPersonRequired",
                table: "SOSSynopticRequirementsOperationSequences",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperationMachineText",
                table: "SOSSynopticRequirementsOperationSequences",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperationPersonText",
                table: "SOSSynopticRequirementsOperationSequences",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SosHubId",
                table: "SOSSynopticRequirementsOperationSequences",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "EntryDate",
                value: new DateTime(2025, 9, 26, 0, 59, 7, 168, DateTimeKind.Local).AddTicks(5080));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 3,
                column: "EntryDate",
                value: new DateTime(2025, 9, 26, 0, 59, 7, 168, DateTimeKind.Local).AddTicks(5083));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 4,
                column: "EntryDate",
                value: new DateTime(2025, 9, 26, 0, 59, 7, 168, DateTimeKind.Local).AddTicks(5085));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 5,
                column: "EntryDate",
                value: new DateTime(2025, 9, 26, 0, 59, 7, 168, DateTimeKind.Local).AddTicks(5086));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 6,
                column: "EntryDate",
                value: new DateTime(2025, 9, 26, 0, 59, 7, 168, DateTimeKind.Local).AddTicks(5088));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 7,
                column: "EntryDate",
                value: new DateTime(2025, 9, 26, 0, 59, 7, 168, DateTimeKind.Local).AddTicks(5090));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 8,
                column: "EntryDate",
                value: new DateTime(2025, 9, 26, 0, 59, 7, 168, DateTimeKind.Local).AddTicks(5092));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 9,
                column: "EntryDate",
                value: new DateTime(2025, 9, 26, 0, 59, 7, 168, DateTimeKind.Local).AddTicks(5094));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 26, 0, 59, 7, 168, DateTimeKind.Local).AddTicks(4672));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 26, 0, 59, 7, 168, DateTimeKind.Local).AddTicks(4696));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 26, 0, 59, 7, 168, DateTimeKind.Local).AddTicks(4698));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 26, 0, 59, 7, 168, DateTimeKind.Local).AddTicks(4700));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 26, 0, 59, 7, 168, DateTimeKind.Local).AddTicks(4704));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 26, 0, 59, 7, 168, DateTimeKind.Local).AddTicks(4707));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 26, 0, 59, 7, 168, DateTimeKind.Local).AddTicks(4709));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 26, 0, 59, 7, 168, DateTimeKind.Local).AddTicks(4711));

            migrationBuilder.AddForeignKey(
                name: "FK_SOSSynopticRequirementsOperationSequences_SOSSynopticTableofOperatingRequirements_SOSSynopticTableofOperatingRequirementsId",
                table: "SOSSynopticRequirementsOperationSequences",
                column: "SOSSynopticTableofOperatingRequirementsId",
                principalTable: "SOSSynopticTableofOperatingRequirements",
                principalColumn: "SOSSynopticTableofOperatingRequirementsId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SOSSynopticRequirementsOperationSequences_SOSSynopticTableofOperatingRequirements_SOSSynopticTableofOperatingRequirementsId",
                table: "SOSSynopticRequirementsOperationSequences");

            migrationBuilder.DropColumn(
                name: "IsOperationMachineRequired",
                table: "SOSSynopticRequirementsOperationSequences");

            migrationBuilder.DropColumn(
                name: "IsOperationPersonRequired",
                table: "SOSSynopticRequirementsOperationSequences");

            migrationBuilder.DropColumn(
                name: "OperationMachineText",
                table: "SOSSynopticRequirementsOperationSequences");

            migrationBuilder.DropColumn(
                name: "OperationPersonText",
                table: "SOSSynopticRequirementsOperationSequences");

            migrationBuilder.DropColumn(
                name: "SosHubId",
                table: "SOSSynopticRequirementsOperationSequences");

            migrationBuilder.AlterColumn<int>(
                name: "SOSSynopticTableofOperatingRequirementsId",
                table: "SOSSynopticRequirementsOperationSequences",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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

            migrationBuilder.AddForeignKey(
                name: "FK_SOSSynopticRequirementsOperationSequences_SOSSynopticTableofOperatingRequirements_SOSSynopticTableofOperatingRequirementsId",
                table: "SOSSynopticRequirementsOperationSequences",
                column: "SOSSynopticTableofOperatingRequirementsId",
                principalTable: "SOSSynopticTableofOperatingRequirements",
                principalColumn: "SOSSynopticTableofOperatingRequirementsId");
        }
    }
}
