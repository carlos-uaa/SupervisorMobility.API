using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserRelationInHRI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HRIs_Users_UserId",
                table: "HRIs");

            migrationBuilder.DropIndex(
                name: "IX_HRIs_UserId",
                table: "HRIs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "HRIs");

            migrationBuilder.DropColumn(
                name: "UserType",
                table: "HRICycles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "HRIs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserType",
                table: "HRICycles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HRIs_UserId",
                table: "HRIs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_HRIs_Users_UserId",
                table: "HRIs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
