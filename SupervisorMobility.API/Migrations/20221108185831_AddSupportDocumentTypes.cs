using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    public partial class AddSupportDocumentTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupportDocumentTypes",
                columns: table => new
                {
                    SupportDocumentTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportDocumentTypes", x => x.SupportDocumentTypeId);
                });

            migrationBuilder.InsertData(
                table: "SupportDocumentTypes",
                columns: new[] { "SupportDocumentTypeId", "Code", "Description", "IsActive" },
                values: new object[] { 1, "GOS", "GOS", true });

            migrationBuilder.InsertData(
                table: "SupportDocumentTypes",
                columns: new[] { "SupportDocumentTypeId", "Code", "Description", "IsActive" },
                values: new object[] { 2, "HOE", "HOE", true });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupportDocumentTypes");
        }
    }
}
