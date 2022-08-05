using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    public partial class groupsSeeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "GroupId", "Code", "Description", "IsActive" },
                values: new object[] { 1, "GA", "Grupo A", true });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "GroupId", "Code", "Description", "IsActive" },
                values: new object[] { 2, "GB", "Grupo B", true });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "GroupId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "GroupId",
                keyValue: 2);
        }
    }
}
