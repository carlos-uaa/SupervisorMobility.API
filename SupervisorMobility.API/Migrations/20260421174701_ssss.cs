using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class ssss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HRICycles_Users_UserId",
                table: "HRICycles");

            migrationBuilder.DropIndex(
                name: "IX_HRICycles_UserId",
                table: "HRICycles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "HRICycles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "HRICycles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HRICycles_UserId",
                table: "HRICycles",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_HRICycles_Users_UserId",
                table: "HRICycles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
