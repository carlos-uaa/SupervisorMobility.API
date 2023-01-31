using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class ProductOperation1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_ProductDistributions_ProductDistributionId",
                table: "Operations");

            migrationBuilder.DropIndex(
                name: "IX_Operations_ProductDistributionId",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "ProductDistributionId",
                table: "Operations");

            migrationBuilder.CreateTable(
                name: "ProductOperations",
                columns: table => new
                {
                    ProductOperationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    ProductDistributionId = table.Column<int>(type: "int", nullable: false)
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
                table: "ProductOperations",
                columns: new[] { "ProductOperationId", "Code", "Description", "IsActive", "ProductDistributionId" },
                values: new object[] { 1, "OP1", "Operation from products", true, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_ProductOperations_ProductDistributionId",
                table: "ProductOperations",
                column: "ProductDistributionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductOperations");

            migrationBuilder.AddColumn<int>(
                name: "ProductDistributionId",
                table: "Operations",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Operations",
                keyColumn: "OperationId",
                keyValue: 1,
                column: "ProductDistributionId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Operations_ProductDistributionId",
                table: "Operations",
                column: "ProductDistributionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_ProductDistributions_ProductDistributionId",
                table: "Operations",
                column: "ProductDistributionId",
                principalTable: "ProductDistributions",
                principalColumn: "ProductDistributionId");
        }
    }
}
