using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    public partial class addIndexChecklistQuestionsAndConstraintSequence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_cq_cod",
                table: "ChecklistQuestions",
                column: "Code",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_cq_seq",
                table: "ChecklistQuestions",
                sql: "[CategorySequence] > 0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_cq_cod",
                table: "ChecklistQuestions");

            migrationBuilder.DropCheckConstraint(
                name: "ck_cq_seq",
                table: "ChecklistQuestions");
        }
    }
}
