using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    public partial class checklistCategorySequenceUniquenessAndConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_cc_cod",
                table: "ChecklistCategories",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cc_seq",
                table: "ChecklistCategories",
                column: "Sequence",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_cc_seq",
                table: "ChecklistCategories",
                sql: "[Sequence] > 0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_cc_cod",
                table: "ChecklistCategories");

            migrationBuilder.DropIndex(
                name: "ix_cc_seq",
                table: "ChecklistCategories");

            migrationBuilder.DropCheckConstraint(
                name: "ck_cc_seq",
                table: "ChecklistCategories");
        }
    }
}
