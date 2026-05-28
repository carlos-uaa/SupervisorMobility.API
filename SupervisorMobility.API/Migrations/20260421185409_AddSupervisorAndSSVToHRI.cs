using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSupervisorAndSSVToHRI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SSVUserId",
                table: "HRIs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupervisorUserId",
                table: "HRIs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HRIs_SSVUserId",
                table: "HRIs",
                column: "SSVUserId");

            migrationBuilder.CreateIndex(
                name: "IX_HRIs_SupervisorUserId",
                table: "HRIs",
                column: "SupervisorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_HRIs_Users_SSVUserId",
                table: "HRIs",
                column: "SSVUserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_HRIs_Users_SupervisorUserId",
                table: "HRIs",
                column: "SupervisorUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HRIs_Users_SSVUserId",
                table: "HRIs");

            migrationBuilder.DropForeignKey(
                name: "FK_HRIs_Users_SupervisorUserId",
                table: "HRIs");

            migrationBuilder.DropIndex(
                name: "IX_HRIs_SSVUserId",
                table: "HRIs");

            migrationBuilder.DropIndex(
                name: "IX_HRIs_SupervisorUserId",
                table: "HRIs");

            migrationBuilder.DropColumn(
                name: "SSVUserId",
                table: "HRIs");

            migrationBuilder.DropColumn(
                name: "SupervisorUserId",
                table: "HRIs");
        }
    }
}
