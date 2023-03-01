using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class AddGlosary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Glosaries",
                table: "Glosaries");

            migrationBuilder.RenameTable(
                name: "Glosaries",
                newName: "Glosary");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Glosary",
                table: "Glosary",
                column: "GlosaryWordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Glosary",
                table: "Glosary");

            migrationBuilder.RenameTable(
                name: "Glosary",
                newName: "Glosaries");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Glosaries",
                table: "Glosaries",
                column: "GlosaryWordId");
        }
    }
}
