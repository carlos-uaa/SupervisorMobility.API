using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoryActionTale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HRIHistoryActions",
                columns: table => new
                {
                    HistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HRIid = table.Column<int>(type: "int", nullable: false),
                    ResponsibleUserId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRIHistoryActions", x => x.HistoryId);
                    table.ForeignKey(
                        name: "FK_HRIHistoryActions_HRIs_HRIid",
                        column: x => x.HRIid,
                        principalTable: "HRIs",
                        principalColumn: "HriId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HRIHistoryActions_Users_ResponsibleUserId",
                        column: x => x.ResponsibleUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_HRIHistoryActions_HRIid",
                table: "HRIHistoryActions",
                column: "HRIid");

            migrationBuilder.CreateIndex(
                name: "IX_HRIHistoryActions_ResponsibleUserId",
                table: "HRIHistoryActions",
                column: "ResponsibleUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HRIHistoryActions");
        }
    }
}
