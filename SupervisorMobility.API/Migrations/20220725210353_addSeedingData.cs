using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    public partial class addSeedingData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ChecklistQuestions",
                columns: new[] { "QuestionID", "AnswerSetID", "CategorySequence", "ChecklistCategoryId", "Code", "Description", "IsActive", "Prompt", "QuestionTypeId" },
                values: new object[] { 2, null, 2, 1, "PO:NIO", "Nivel ILU del operador", true, "¿Cuál es nivel de ILU del operador?  ¿Está el entrenamiento alineado con el Cuadro de requisitos de Operaicón ? (S/N)", 6 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ChecklistQuestions",
                keyColumn: "QuestionID",
                keyValue: 2);
        }
    }
}
