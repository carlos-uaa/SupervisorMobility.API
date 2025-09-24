using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class addSkillAndKnowledgeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Knowledge",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Knowledge", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skill",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skill", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SOSSTROKnowledgeHub",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KnowledgeId = table.Column<int>(type: "int", nullable: false),
                    SOSHubId = table.Column<int>(type: "int", nullable: false),
                    SOSSynopticTableofOperatingRequirementsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOSSTROKnowledgeHub", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SOSSTROKnowledgeHub_Knowledge_KnowledgeId",
                        column: x => x.KnowledgeId,
                        principalTable: "Knowledge",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SOSSTROKnowledgeHub_SOSHubs_SOSHubId",
                        column: x => x.SOSHubId,
                        principalTable: "SOSHubs",
                        principalColumn: "SOSHubId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SOSSTROKnowledgeHub_SOSSynopticTableofOperatingRequirements_SOSSynopticTableofOperatingRequirementsId",
                        column: x => x.SOSSynopticTableofOperatingRequirementsId,
                        principalTable: "SOSSynopticTableofOperatingRequirements",
                        principalColumn: "SOSSynopticTableofOperatingRequirementsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SOSSTROSkillHub",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    SOSHubId = table.Column<int>(type: "int", nullable: false),
                    SOSSynopticTableofOperatingRequirementsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOSSTROSkillHub", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SOSSTROSkillHub_SOSHubs_SOSHubId",
                        column: x => x.SOSHubId,
                        principalTable: "SOSHubs",
                        principalColumn: "SOSHubId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SOSSTROSkillHub_SOSSynopticTableofOperatingRequirements_SOSSynopticTableofOperatingRequirementsId",
                        column: x => x.SOSSynopticTableofOperatingRequirementsId,
                        principalTable: "SOSSynopticTableofOperatingRequirements",
                        principalColumn: "SOSSynopticTableofOperatingRequirementsId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SOSSTROSkillHub_Skill_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_SOSSTROKnowledgeHub_KnowledgeId",
                table: "SOSSTROKnowledgeHub",
                column: "KnowledgeId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSTROKnowledgeHub_SOSHubId",
                table: "SOSSTROKnowledgeHub",
                column: "SOSHubId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSTROKnowledgeHub_SOSSynopticTableofOperatingRequirementsId",
                table: "SOSSTROKnowledgeHub",
                column: "SOSSynopticTableofOperatingRequirementsId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSTROSkillHub_SkillId",
                table: "SOSSTROSkillHub",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSTROSkillHub_SOSHubId",
                table: "SOSSTROSkillHub",
                column: "SOSHubId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSTROSkillHub_SOSSynopticTableofOperatingRequirementsId",
                table: "SOSSTROSkillHub",
                column: "SOSSynopticTableofOperatingRequirementsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SOSSTROKnowledgeHub");

            migrationBuilder.DropTable(
                name: "SOSSTROSkillHub");

            migrationBuilder.DropTable(
                name: "Knowledge");

            migrationBuilder.DropTable(
                name: "Skill");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 11, 45, 32, 594, DateTimeKind.Local).AddTicks(5523));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 3,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 11, 45, 32, 594, DateTimeKind.Local).AddTicks(5525));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 4,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 11, 45, 32, 594, DateTimeKind.Local).AddTicks(5527));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 5,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 11, 45, 32, 594, DateTimeKind.Local).AddTicks(5529));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 6,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 11, 45, 32, 594, DateTimeKind.Local).AddTicks(5531));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 7,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 11, 45, 32, 594, DateTimeKind.Local).AddTicks(5533));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 8,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 11, 45, 32, 594, DateTimeKind.Local).AddTicks(5534));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 9,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 11, 45, 32, 594, DateTimeKind.Local).AddTicks(5536));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 11, 45, 32, 594, DateTimeKind.Local).AddTicks(5299));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 11, 45, 32, 594, DateTimeKind.Local).AddTicks(5315));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 11, 45, 32, 594, DateTimeKind.Local).AddTicks(5317));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 11, 45, 32, 594, DateTimeKind.Local).AddTicks(5318));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 11, 45, 32, 594, DateTimeKind.Local).AddTicks(5322));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 11, 45, 32, 594, DateTimeKind.Local).AddTicks(5325));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 11, 45, 32, 594, DateTimeKind.Local).AddTicks(5327));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 11, 45, 32, 594, DateTimeKind.Local).AddTicks(5330));
        }
    }
}
