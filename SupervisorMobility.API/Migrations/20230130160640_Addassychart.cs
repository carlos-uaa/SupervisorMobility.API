using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class Addassychart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Operations",
                columns: table => new
                {
                    OperationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    DistributionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operations", x => x.OperationId);
                    table.ForeignKey(
                        name: "FK_Operations_Distributions_DistributionId",
                        column: x => x.DistributionId,
                        principalTable: "Distributions",
                        principalColumn: "DistributionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "AssyCharts",
                columns: table => new
                {
                    AssyChardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    GOS = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CCP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HOE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "Date", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "Date", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    PlantId = table.Column<int>(type: "int", nullable: true),
                    AreaId = table.Column<int>(type: "int", nullable: true),
                    DistributionId = table.Column<int>(type: "int", nullable: true),
                    OperationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssyCharts", x => x.AssyChardId);
                    table.ForeignKey(
                        name: "FK_AssyCharts_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "AreaId");
                    table.ForeignKey(
                        name: "FK_AssyCharts_Distributions_DistributionId",
                        column: x => x.DistributionId,
                        principalTable: "Distributions",
                        principalColumn: "DistributionId");
                    table.ForeignKey(
                        name: "FK_AssyCharts_Operations_OperationId",
                        column: x => x.OperationId,
                        principalTable: "Operations",
                        principalColumn: "OperationId");
                    table.ForeignKey(
                        name: "FK_AssyCharts_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "PlantId");
                    table.ForeignKey(
                        name: "FK_AssyCharts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                });

            migrationBuilder.InsertData(
                table: "Operations",
                columns: new[] { "OperationId", "Code", "Description", "DistributionId", "IsActive" },
                values: new object[] { 1, "OP1", "Operacion Trim 1", 1, true });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Code", "Description", "IsActive" },
                values: new object[,]
                {
                    { 1, "P71A", "Infiniti P71A", true },
                    { 3, "X247", "Mercedes X247", true }
                });

            migrationBuilder.InsertData(
                table: "AssyCharts",
                columns: new[] { "AssyChardId", "AreaId", "CCP", "CreationDate", "DistributionId", "GOS", "HOE", "IsActive", "ModificationDate", "OperationId", "PlantId", "ProductId" },
                values: new object[] { 1, 1, "string", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "string", "string", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_AssyCharts_AreaId",
                table: "AssyCharts",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_AssyCharts_DistributionId",
                table: "AssyCharts",
                column: "DistributionId");

            migrationBuilder.CreateIndex(
                name: "IX_AssyCharts_OperationId",
                table: "AssyCharts",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_AssyCharts_PlantId",
                table: "AssyCharts",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_AssyCharts_ProductId",
                table: "AssyCharts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_DistributionId",
                table: "Operations",
                column: "DistributionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssyCharts");

            migrationBuilder.DropTable(
                name: "Operations");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
