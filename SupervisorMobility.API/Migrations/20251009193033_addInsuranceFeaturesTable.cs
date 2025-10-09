using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class addInsuranceFeaturesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InsuranceFeatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Insurance = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SOSSynopticTableofOperatingRequirementsId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceFeatures_SOSSynopticTableofOperatingRequirements_SOSSynopticTableofOperatingRequirementsId",
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

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceFeatures_SOSSynopticTableofOperatingRequirementsId",
                table: "InsuranceFeatures",
                column: "SOSSynopticTableofOperatingRequirementsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InsuranceFeatures");

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
        }
    }
}
