using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    public partial class SupervisorMobilityInitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChecklistCategories",
                columns: table => new
                {
                    ChecklistCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistCategories", x => x.ChecklistCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "QuestionTypes",
                columns: table => new
                {
                    QuestionTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionTypes", x => x.QuestionTypeId);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistQuestions",
                columns: table => new
                {
                    QuestionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Prompt = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CategorySequence = table.Column<int>(type: "int", nullable: false),
                    AnswerSetID = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    ChecklistCategoryId = table.Column<int>(type: "int", nullable: false),
                    QuestionTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistQuestions", x => x.QuestionID);
                    table.ForeignKey(
                        name: "FK_ChecklistQuestions_ChecklistCategories_ChecklistCategoryId",
                        column: x => x.ChecklistCategoryId,
                        principalTable: "ChecklistCategories",
                        principalColumn: "ChecklistCategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChecklistQuestions_QuestionTypes_QuestionTypeId",
                        column: x => x.QuestionTypeId,
                        principalTable: "QuestionTypes",
                        principalColumn: "QuestionTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ChecklistCategories",
                columns: new[] { "ChecklistCategoryId", "Code", "Description", "IsActive", "Sequence" },
                values: new object[,]
                {
                    { 1, "PO", "Preparación de la Observación", true, 1 },
                    { 2, "OPCE", "Observación para el cumplimiento del estándar - Observación de lejos", true, 2 },
                    { 3, "ATO", "Análisis de tiempo de operación", true, 3 },
                    { 4, "OCE", "Observación para cumplimiento del estándar - Observación de cerca", true, 4 },
                    { 5, "OMEFE", "Observación para mejora del estándar de acuerdo al filtro elegido", true, 5 },
                    { 6, "TOSF", "Trabajo de Observación  - Sumario / Finalización", true, 6 }
                });

            migrationBuilder.InsertData(
                table: "QuestionTypes",
                columns: new[] { "QuestionTypeId", "Code", "Description", "IsActive" },
                values: new object[,]
                {
                    { 1, "TXT", "Free text", true },
                    { 2, "MC", "Multiple Choice", true },
                    { 3, "NMB", "Number", true },
                    { 4, "Date", "Date", true },
                    { 5, "TM", "Time", true },
                    { 6, "TF", "Si/No", true }
                });

            migrationBuilder.InsertData(
                table: "ChecklistQuestions",
                columns: new[] { "QuestionID", "AnswerSetID", "CategorySequence", "ChecklistCategoryId", "Code", "Description", "IsActive", "Prompt", "QuestionTypeId" },
                values: new object[] { 1, null, 1, 1, "PO:ECA", "Estandares completos y actualizados", true, "Los estándares estan completos y actualizados (HOE, Estado de referencia de 5S, etc. Icluyendo la pasada observación de operación  (S/N)", 6 });

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistQuestions_ChecklistCategoryId",
                table: "ChecklistQuestions",
                column: "ChecklistCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistQuestions_QuestionTypeId",
                table: "ChecklistQuestions",
                column: "QuestionTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChecklistQuestions");

            migrationBuilder.DropTable(
                name: "ChecklistCategories");

            migrationBuilder.DropTable(
                name: "QuestionTypes");
        }
    }
}
