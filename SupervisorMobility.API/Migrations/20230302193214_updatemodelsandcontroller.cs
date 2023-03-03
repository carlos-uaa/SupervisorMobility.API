using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class updatemodelsandcontroller : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductOperations");

            migrationBuilder.DropTable(
                name: "ProductDistributions");

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

            migrationBuilder.CreateTable(
                name: "DistributionProduct",
                columns: table => new
                {
                    DistributionsDistributionId = table.Column<int>(type: "int", nullable: false),
                    ProductsProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistributionProduct", x => new { x.DistributionsDistributionId, x.ProductsProductId });
                    table.ForeignKey(
                        name: "FK_DistributionProduct_Distributions_DistributionsDistributionId",
                        column: x => x.DistributionsDistributionId,
                        principalTable: "Distributions",
                        principalColumn: "DistributionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DistributionProduct_Products_ProductsProductId",
                        column: x => x.ProductsProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Payroll = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    IsSupervisor = table.Column<bool>(type: "bit", nullable: false),
                    IsOperator = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "Date", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "Date", nullable: false),
                    DisabledDate = table.Column<DateTime>(type: "Date", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    PlantId = table.Column<int>(type: "int", nullable: true),
                    AreaId = table.Column<int>(type: "int", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "AreaId");
                    table.ForeignKey(
                        name: "FK_Users_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "FK_Users_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "PlantId");
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "AreaId", "CreatedDate", "DisabledDate", "GroupId", "IsActive", "IsAdmin", "IsOperator", "IsSupervisor", "LastUpdated", "Name", "Payroll", "PlantId" },
                values: new object[] { 1, 1, null, null, 1, true, true, false, true, new DateTime(2023, 3, 2, 13, 32, 13, 940, DateTimeKind.Local).AddTicks(4891), "Marco Aguayo", 239935, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_DistributionProduct_ProductsProductId",
                table: "DistributionProduct",
                column: "ProductsProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AreaId",
                table: "Users",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_GroupId",
                table: "Users",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PlantId",
                table: "Users",
                column: "PlantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DistributionProduct");

            migrationBuilder.DropTable(
                name: "Users");

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

            migrationBuilder.CreateTable(
                name: "ProductDistributions",
                columns: table => new
                {
                    ProductDistributionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDistributions", x => x.ProductDistributionId);
                    table.ForeignKey(
                        name: "FK_ProductDistributions_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductOperations",
                columns: table => new
                {
                    ProductOperationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductDistributionId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductOperations", x => x.ProductOperationId);
                    table.ForeignKey(
                        name: "FK_ProductOperations_ProductDistributions_ProductDistributionId",
                        column: x => x.ProductDistributionId,
                        principalTable: "ProductDistributions",
                        principalColumn: "ProductDistributionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ProductDistributions",
                columns: new[] { "ProductDistributionId", "Code", "Description", "IsActive", "ProductId" },
                values: new object[] { 1, "Dist1", "Distribution from products", true, 1 });

            migrationBuilder.InsertData(
                table: "ProductOperations",
                columns: new[] { "ProductOperationId", "Code", "Description", "IsActive", "ProductDistributionId" },
                values: new object[] { 1, "OP1", "Operation from products", true, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_ProductDistributions_ProductId",
                table: "ProductDistributions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductOperations_ProductDistributionId",
                table: "ProductOperations",
                column: "ProductDistributionId");
        }
    }
}
