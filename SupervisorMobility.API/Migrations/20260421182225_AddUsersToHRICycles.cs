using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersToHRICycles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OperatorUserId",
                table: "HRICycles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupervisorUserId",
                table: "HRICycles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HRICycles_OperatorUserId",
                table: "HRICycles",
                column: "OperatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_HRICycles_SupervisorUserId",
                table: "HRICycles",
                column: "SupervisorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_HRICycles_Users_OperatorUserId",
                table: "HRICycles",
                column: "OperatorUserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_HRICycles_Users_SupervisorUserId",
                table: "HRICycles",
                column: "SupervisorUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HRICycles_Users_OperatorUserId",
                table: "HRICycles");

            migrationBuilder.DropForeignKey(
                name: "FK_HRICycles_Users_SupervisorUserId",
                table: "HRICycles");

            migrationBuilder.DropIndex(
                name: "IX_HRICycles_OperatorUserId",
                table: "HRICycles");

            migrationBuilder.DropIndex(
                name: "IX_HRICycles_SupervisorUserId",
                table: "HRICycles");

            migrationBuilder.DropColumn(
                name: "OperatorUserId",
                table: "HRICycles");

            migrationBuilder.DropColumn(
                name: "SupervisorUserId",
                table: "HRICycles");
        }
    }
}
