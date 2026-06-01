using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class LocalUserCourses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocalUserCourses",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reticulate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Calification = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HCIId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalUserCourses", x => x.CourseId);
                    table.ForeignKey(
                        name: "FK_LocalUserCourses_HCIs_HCIId",
                        column: x => x.HCIId,
                        principalTable: "HCIs",
                        principalColumn: "HCIId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "EntryDate",
                value: new DateTime(2026, 3, 11, 9, 56, 52, 985, DateTimeKind.Local).AddTicks(853));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 3,
                column: "EntryDate",
                value: new DateTime(2026, 3, 11, 9, 56, 52, 985, DateTimeKind.Local).AddTicks(856));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 4,
                column: "EntryDate",
                value: new DateTime(2026, 3, 11, 9, 56, 52, 985, DateTimeKind.Local).AddTicks(862));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 5,
                column: "EntryDate",
                value: new DateTime(2026, 3, 11, 9, 56, 52, 985, DateTimeKind.Local).AddTicks(864));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 6,
                column: "EntryDate",
                value: new DateTime(2026, 3, 11, 9, 56, 52, 985, DateTimeKind.Local).AddTicks(883));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 7,
                column: "EntryDate",
                value: new DateTime(2026, 3, 11, 9, 56, 52, 985, DateTimeKind.Local).AddTicks(888));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 8,
                column: "EntryDate",
                value: new DateTime(2026, 3, 11, 9, 56, 52, 985, DateTimeKind.Local).AddTicks(905));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 9,
                column: "EntryDate",
                value: new DateTime(2026, 3, 11, 9, 56, 52, 985, DateTimeKind.Local).AddTicks(907));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 11, 9, 56, 52, 984, DateTimeKind.Local).AddTicks(6043));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 11, 9, 56, 52, 984, DateTimeKind.Local).AddTicks(6763));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 11, 9, 56, 52, 984, DateTimeKind.Local).AddTicks(6766));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 11, 9, 56, 52, 984, DateTimeKind.Local).AddTicks(6767));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 11, 9, 56, 52, 984, DateTimeKind.Local).AddTicks(7327));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 11, 9, 56, 52, 984, DateTimeKind.Local).AddTicks(7427));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 11, 9, 56, 52, 984, DateTimeKind.Local).AddTicks(7430));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 11, 9, 56, 52, 984, DateTimeKind.Local).AddTicks(7433));

            migrationBuilder.CreateIndex(
                name: "IX_LocalUserCourses_HCIId",
                table: "LocalUserCourses",
                column: "HCIId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocalUserCourses");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "EntryDate",
                value: new DateTime(2026, 2, 13, 15, 12, 27, 969, DateTimeKind.Local).AddTicks(2361));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 3,
                column: "EntryDate",
                value: new DateTime(2026, 2, 13, 15, 12, 27, 969, DateTimeKind.Local).AddTicks(2364));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 4,
                column: "EntryDate",
                value: new DateTime(2026, 2, 13, 15, 12, 27, 969, DateTimeKind.Local).AddTicks(2366));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 5,
                column: "EntryDate",
                value: new DateTime(2026, 2, 13, 15, 12, 27, 969, DateTimeKind.Local).AddTicks(2368));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 6,
                column: "EntryDate",
                value: new DateTime(2026, 2, 13, 15, 12, 27, 969, DateTimeKind.Local).AddTicks(2370));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 7,
                column: "EntryDate",
                value: new DateTime(2026, 2, 13, 15, 12, 27, 969, DateTimeKind.Local).AddTicks(2372));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 8,
                column: "EntryDate",
                value: new DateTime(2026, 2, 13, 15, 12, 27, 969, DateTimeKind.Local).AddTicks(2374));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 9,
                column: "EntryDate",
                value: new DateTime(2026, 2, 13, 15, 12, 27, 969, DateTimeKind.Local).AddTicks(2376));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 13, 15, 12, 27, 969, DateTimeKind.Local).AddTicks(2150));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 13, 15, 12, 27, 969, DateTimeKind.Local).AddTicks(2160));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 13, 15, 12, 27, 969, DateTimeKind.Local).AddTicks(2162));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 13, 15, 12, 27, 969, DateTimeKind.Local).AddTicks(2163));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 13, 15, 12, 27, 969, DateTimeKind.Local).AddTicks(2167));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 13, 15, 12, 27, 969, DateTimeKind.Local).AddTicks(2173));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 13, 15, 12, 27, 969, DateTimeKind.Local).AddTicks(2176));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 13, 15, 12, 27, 969, DateTimeKind.Local).AddTicks(2179));
        }
    }
}
