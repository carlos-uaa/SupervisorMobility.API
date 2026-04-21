using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantAndAreaToHRI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AreaId",
                table: "HRIs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlantId",
                table: "HRIs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HRIs_AreaId",
                table: "HRIs",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_HRIs_PlantId",
                table: "HRIs",
                column: "PlantId");

            migrationBuilder.AddForeignKey(
                name: "FK_HRIs_Areas_AreaId",
                table: "HRIs",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "AreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_HRIs_Plants_PlantId",
                table: "HRIs",
                column: "PlantId",
                principalTable: "Plants",
                principalColumn: "PlantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HRIs_Areas_AreaId",
                table: "HRIs");

            migrationBuilder.DropForeignKey(
                name: "FK_HRIs_Plants_PlantId",
                table: "HRIs");

            migrationBuilder.DropIndex(
                name: "IX_HRIs_AreaId",
                table: "HRIs");

            migrationBuilder.DropIndex(
                name: "IX_HRIs_PlantId",
                table: "HRIs");

            migrationBuilder.DropColumn(
                name: "AreaId",
                table: "HRIs");

            migrationBuilder.DropColumn(
                name: "PlantId",
                table: "HRIs");
        }
    }
}
