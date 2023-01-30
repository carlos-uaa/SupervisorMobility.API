using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class ProductDistribution1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AssyCharts",
                keyColumn: "AssyChardId",
                keyValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "ProductDistributionDistributionId",
                table: "Operations",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "AssyCharts",
                type: "bit",
                nullable: true,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ProductDistributions",
                columns: table => new
                {
                    DistributionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDistributions", x => x.DistributionId);
                    table.ForeignKey(
                        name: "FK_ProductDistributions_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Operations",
                keyColumn: "OperationId",
                keyValue: 1,
                column: "ProductDistributionDistributionId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Operations_ProductDistributionDistributionId",
                table: "Operations",
                column: "ProductDistributionDistributionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDistributions_ProductId",
                table: "ProductDistributions",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_ProductDistributions_ProductDistributionDistributionId",
                table: "Operations",
                column: "ProductDistributionDistributionId",
                principalTable: "ProductDistributions",
                principalColumn: "DistributionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_ProductDistributions_ProductDistributionDistributionId",
                table: "Operations");

            migrationBuilder.DropTable(
                name: "ProductDistributions");

            migrationBuilder.DropIndex(
                name: "IX_Operations_ProductDistributionDistributionId",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "ProductDistributionDistributionId",
                table: "Operations");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "AssyCharts",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValue: true);

            migrationBuilder.InsertData(
                table: "AssyCharts",
                columns: new[] { "AssyChardId", "AreaId", "CCP", "CreationDate", "DistributionId", "GOS", "HOE", "IsActive", "ModificationDate", "OperationId", "PlantId", "ProductId" },
                values: new object[] { 1, 1, "string", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "string", "string", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 1 });
        }
    }
}
