using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class addEstablishedConditionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EstablishedConditions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Condition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SOSSynopticTableofOperatingRequirementsId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstablishedConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstablishedConditions_SOSSynopticTableofOperatingRequirements_SOSSynopticTableofOperatingRequirementsId",
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

            migrationBuilder.CreateIndex(
                name: "IX_EstablishedConditions_SOSSynopticTableofOperatingRequirementsId",
                table: "EstablishedConditions",
                column: "SOSSynopticTableofOperatingRequirementsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EstablishedConditions");

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
        }
    }
}
