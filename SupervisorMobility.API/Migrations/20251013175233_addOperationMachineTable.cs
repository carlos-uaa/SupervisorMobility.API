using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class addOperationMachineTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OperationMachine",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Operation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SOSSynopticTableofOperatingRequirementsId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationMachine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationMachine_SOSSynopticTableofOperatingRequirements_SOSSynopticTableofOperatingRequirementsId",
                        column: x => x.SOSSynopticTableofOperatingRequirementsId,
                        principalTable: "SOSSynopticTableofOperatingRequirements",
                        principalColumn: "SOSSynopticTableofOperatingRequirementsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "EntryDate",
                value: new DateTime(2025, 10, 13, 11, 52, 31, 608, DateTimeKind.Local).AddTicks(4379));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 3,
                column: "EntryDate",
                value: new DateTime(2025, 10, 13, 11, 52, 31, 608, DateTimeKind.Local).AddTicks(4381));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 4,
                column: "EntryDate",
                value: new DateTime(2025, 10, 13, 11, 52, 31, 608, DateTimeKind.Local).AddTicks(4383));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 5,
                column: "EntryDate",
                value: new DateTime(2025, 10, 13, 11, 52, 31, 608, DateTimeKind.Local).AddTicks(4385));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 6,
                column: "EntryDate",
                value: new DateTime(2025, 10, 13, 11, 52, 31, 608, DateTimeKind.Local).AddTicks(4387));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 7,
                column: "EntryDate",
                value: new DateTime(2025, 10, 13, 11, 52, 31, 608, DateTimeKind.Local).AddTicks(4388));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 8,
                column: "EntryDate",
                value: new DateTime(2025, 10, 13, 11, 52, 31, 608, DateTimeKind.Local).AddTicks(4390));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 9,
                column: "EntryDate",
                value: new DateTime(2025, 10, 13, 11, 52, 31, 608, DateTimeKind.Local).AddTicks(4392));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 13, 11, 52, 31, 608, DateTimeKind.Local).AddTicks(4171));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 13, 11, 52, 31, 608, DateTimeKind.Local).AddTicks(4187));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 13, 11, 52, 31, 608, DateTimeKind.Local).AddTicks(4189));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 13, 11, 52, 31, 608, DateTimeKind.Local).AddTicks(4191));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 13, 11, 52, 31, 608, DateTimeKind.Local).AddTicks(4195));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 13, 11, 52, 31, 608, DateTimeKind.Local).AddTicks(4198));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 13, 11, 52, 31, 608, DateTimeKind.Local).AddTicks(4201));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 13, 11, 52, 31, 608, DateTimeKind.Local).AddTicks(4203));

            migrationBuilder.CreateIndex(
                name: "IX_OperationMachine_SOSSynopticTableofOperatingRequirementsId",
                table: "OperationMachine",
                column: "SOSSynopticTableofOperatingRequirementsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperationMachine");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "EntryDate",
                value: new DateTime(2025, 10, 9, 13, 30, 32, 250, DateTimeKind.Local).AddTicks(1666));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 3,
                column: "EntryDate",
                value: new DateTime(2025, 10, 9, 13, 30, 32, 250, DateTimeKind.Local).AddTicks(1669));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 4,
                column: "EntryDate",
                value: new DateTime(2025, 10, 9, 13, 30, 32, 250, DateTimeKind.Local).AddTicks(1672));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 5,
                column: "EntryDate",
                value: new DateTime(2025, 10, 9, 13, 30, 32, 250, DateTimeKind.Local).AddTicks(1675));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 6,
                column: "EntryDate",
                value: new DateTime(2025, 10, 9, 13, 30, 32, 250, DateTimeKind.Local).AddTicks(1678));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 7,
                column: "EntryDate",
                value: new DateTime(2025, 10, 9, 13, 30, 32, 250, DateTimeKind.Local).AddTicks(1680));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 8,
                column: "EntryDate",
                value: new DateTime(2025, 10, 9, 13, 30, 32, 250, DateTimeKind.Local).AddTicks(1682));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 9,
                column: "EntryDate",
                value: new DateTime(2025, 10, 9, 13, 30, 32, 250, DateTimeKind.Local).AddTicks(1685));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 9, 13, 30, 32, 250, DateTimeKind.Local).AddTicks(1475));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 9, 13, 30, 32, 250, DateTimeKind.Local).AddTicks(1493));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 9, 13, 30, 32, 250, DateTimeKind.Local).AddTicks(1495));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 9, 13, 30, 32, 250, DateTimeKind.Local).AddTicks(1498));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 9, 13, 30, 32, 250, DateTimeKind.Local).AddTicks(1502));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 9, 13, 30, 32, 250, DateTimeKind.Local).AddTicks(1505));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 9, 13, 30, 32, 250, DateTimeKind.Local).AddTicks(1507));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 9, 13, 30, 32, 250, DateTimeKind.Local).AddTicks(1510));
        }
    }
}
