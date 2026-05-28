using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class HRIFirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Frequencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Frequencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HRIDocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DockName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRIDocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HRIItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ControlNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRIItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HRILines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRILines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RevisionMethods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevisionMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Veredicts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veredicts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HRIs",
                columns: table => new
                {
                    HriId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HRILinesId = table.Column<int>(type: "int", nullable: true),
                    HRIItemId = table.Column<int>(type: "int", nullable: true),
                    ControlNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HRIDockId = table.Column<int>(type: "int", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRIs", x => x.HriId);
                    table.ForeignKey(
                        name: "FK_HRIs_HRIDocks_HRIDockId",
                        column: x => x.HRIDockId,
                        principalTable: "HRIDocks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HRIs_HRIItems_HRIItemId",
                        column: x => x.HRIItemId,
                        principalTable: "HRIItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HRIs_HRILines_HRILinesId",
                        column: x => x.HRILinesId,
                        principalTable: "HRILines",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HRIs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "HourmeterRevisions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HriId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HourmeterRevisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HourmeterRevisions_HRIs_HriId",
                        column: x => x.HriId,
                        principalTable: "HRIs",
                        principalColumn: "HriId");
                });

            migrationBuilder.CreateTable(
                name: "HRICycles",
                columns: table => new
                {
                    CycleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cycle = table.Column<int>(type: "int", nullable: false),
                    HriId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    UserType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRICycles", x => x.CycleId);
                    table.ForeignKey(
                        name: "FK_HRICycles_HRIs_HriId",
                        column: x => x.HriId,
                        principalTable: "HRIs",
                        principalColumn: "HriId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HRICycles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "HRImages",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HriId = table.Column<int>(type: "int", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRImages", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_HRImages_HRIs_HriId",
                        column: x => x.HriId,
                        principalTable: "HRIs",
                        principalColumn: "HriId");
                });

            migrationBuilder.CreateTable(
                name: "HRIRevisionItems",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HriId = table.Column<int>(type: "int", nullable: false),
                    ItemNumber = table.Column<int>(type: "int", nullable: false),
                    RevisionPoint = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RevisionMethodId = table.Column<int>(type: "int", nullable: true),
                    VeredictId = table.Column<int>(type: "int", nullable: true),
                    FrequencyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRIRevisionItems", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_HRIRevisionItems_Frequencies_FrequencyId",
                        column: x => x.FrequencyId,
                        principalTable: "Frequencies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HRIRevisionItems_HRIs_HriId",
                        column: x => x.HriId,
                        principalTable: "HRIs",
                        principalColumn: "HriId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HRIRevisionItems_RevisionMethods_RevisionMethodId",
                        column: x => x.RevisionMethodId,
                        principalTable: "RevisionMethods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HRIRevisionItems_Veredicts_VeredictId",
                        column: x => x.VeredictId,
                        principalTable: "Veredicts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WeeklyRevisions",
                columns: table => new
                {
                    RevisionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HriId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Week = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyRevisions", x => x.RevisionId);
                    table.ForeignKey(
                        name: "FK_WeeklyRevisions_HRIs_HriId",
                        column: x => x.HriId,
                        principalTable: "HRIs",
                        principalColumn: "HriId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeeklyRevisions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "RevisionCycles",
                columns: table => new
                {
                    RevisionCycleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cycle = table.Column<int>(type: "int", nullable: false),
                    HRIRevisionItemsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevisionCycles", x => x.RevisionCycleId);
                    table.ForeignKey(
                        name: "FK_RevisionCycles_HRIRevisionItems_HRIRevisionItemsId",
                        column: x => x.HRIRevisionItemsId,
                        principalTable: "HRIRevisionItems",
                        principalColumn: "ItemId");
                });

            migrationBuilder.CreateTable(
                name: "DailyRevisions",
                columns: table => new
                {
                    RevisionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RevisionCycleId = table.Column<int>(type: "int", nullable: true),
                    CycleId = table.Column<int>(type: "int", nullable: true),
                    HourmeterId = table.Column<int>(type: "int", nullable: true),
                    HourmeterRevisionId = table.Column<int>(type: "int", nullable: true),
                    Day = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    UserType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyRevisions", x => x.RevisionId);
                    table.ForeignKey(
                        name: "FK_DailyRevisions_HRICycles_CycleId",
                        column: x => x.CycleId,
                        principalTable: "HRICycles",
                        principalColumn: "CycleId");
                    table.ForeignKey(
                        name: "FK_DailyRevisions_HourmeterRevisions_HourmeterRevisionId",
                        column: x => x.HourmeterRevisionId,
                        principalTable: "HourmeterRevisions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DailyRevisions_RevisionCycles_RevisionCycleId",
                        column: x => x.RevisionCycleId,
                        principalTable: "RevisionCycles",
                        principalColumn: "RevisionCycleId");
                    table.ForeignKey(
                        name: "FK_DailyRevisions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyRevisions_CycleId",
                table: "DailyRevisions",
                column: "CycleId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyRevisions_HourmeterRevisionId",
                table: "DailyRevisions",
                column: "HourmeterRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyRevisions_RevisionCycleId",
                table: "DailyRevisions",
                column: "RevisionCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyRevisions_UserId",
                table: "DailyRevisions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HourmeterRevisions_HriId",
                table: "HourmeterRevisions",
                column: "HriId",
                unique: true,
                filter: "[HriId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HRICycles_HriId",
                table: "HRICycles",
                column: "HriId");

            migrationBuilder.CreateIndex(
                name: "IX_HRICycles_UserId",
                table: "HRICycles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HRImages_HriId",
                table: "HRImages",
                column: "HriId");

            migrationBuilder.CreateIndex(
                name: "IX_HRIRevisionItems_FrequencyId",
                table: "HRIRevisionItems",
                column: "FrequencyId");

            migrationBuilder.CreateIndex(
                name: "IX_HRIRevisionItems_HriId",
                table: "HRIRevisionItems",
                column: "HriId");

            migrationBuilder.CreateIndex(
                name: "IX_HRIRevisionItems_RevisionMethodId",
                table: "HRIRevisionItems",
                column: "RevisionMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_HRIRevisionItems_VeredictId",
                table: "HRIRevisionItems",
                column: "VeredictId");

            migrationBuilder.CreateIndex(
                name: "IX_HRIs_HRIDockId",
                table: "HRIs",
                column: "HRIDockId");

            migrationBuilder.CreateIndex(
                name: "IX_HRIs_HRIItemId",
                table: "HRIs",
                column: "HRIItemId");

            migrationBuilder.CreateIndex(
                name: "IX_HRIs_HRILinesId",
                table: "HRIs",
                column: "HRILinesId");

            migrationBuilder.CreateIndex(
                name: "IX_HRIs_UserId",
                table: "HRIs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RevisionCycles_HRIRevisionItemsId",
                table: "RevisionCycles",
                column: "HRIRevisionItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyRevisions_HriId",
                table: "WeeklyRevisions",
                column: "HriId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyRevisions_UserId",
                table: "WeeklyRevisions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyRevisions");

            migrationBuilder.DropTable(
                name: "HRImages");

            migrationBuilder.DropTable(
                name: "WeeklyRevisions");

            migrationBuilder.DropTable(
                name: "HRICycles");

            migrationBuilder.DropTable(
                name: "HourmeterRevisions");

            migrationBuilder.DropTable(
                name: "RevisionCycles");

            migrationBuilder.DropTable(
                name: "HRIRevisionItems");

            migrationBuilder.DropTable(
                name: "Frequencies");

            migrationBuilder.DropTable(
                name: "HRIs");

            migrationBuilder.DropTable(
                name: "RevisionMethods");

            migrationBuilder.DropTable(
                name: "Veredicts");

            migrationBuilder.DropTable(
                name: "HRIDocks");

            migrationBuilder.DropTable(
                name: "HRIItems");

            migrationBuilder.DropTable(
                name: "HRILines");
        }
    }
}
