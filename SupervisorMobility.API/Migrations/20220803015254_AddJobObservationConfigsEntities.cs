using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    public partial class AddJobObservationConfigsEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobObservationTypes",
                columns: table => new
                {
                    JobObservationTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobObservationTypes", x => x.JobObservationTypeId);
                });

            migrationBuilder.CreateTable(
                name: "JobObservationConfigs",
                columns: table => new
                {
                    JobObservationConfigId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobObservationTypeId = table.Column<int>(type: "int", nullable: false),
                    ChecklistCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobObservationConfigs", x => x.JobObservationConfigId);
                    table.ForeignKey(
                        name: "FK_JobObservationConfigs_ChecklistCategories_ChecklistCategoryId",
                        column: x => x.ChecklistCategoryId,
                        principalTable: "ChecklistCategories",
                        principalColumn: "ChecklistCategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobObservationConfigs_JobObservationTypes_JobObservationTypeId",
                        column: x => x.JobObservationTypeId,
                        principalTable: "JobObservationTypes",
                        principalColumn: "JobObservationTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobObservationConfigs_ChecklistCategoryId",
                table: "JobObservationConfigs",
                column: "ChecklistCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_JobObservationConfigs_JobObservationTypeId",
                table: "JobObservationConfigs",
                column: "JobObservationTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobObservationConfigs");

            migrationBuilder.DropTable(
                name: "JobObservationTypes");
        }
    }
}
