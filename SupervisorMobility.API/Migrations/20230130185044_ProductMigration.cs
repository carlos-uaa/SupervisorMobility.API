using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class ProductMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_ProductDistributions_ProductDistributionDistributionId",
                table: "Operations");

            migrationBuilder.RenameColumn(
                name: "DistributionId",
                table: "ProductDistributions",
                newName: "ProductDistributionId");

            migrationBuilder.RenameColumn(
                name: "ProductDistributionDistributionId",
                table: "Operations",
                newName: "ProductDistributionId");

            migrationBuilder.RenameIndex(
                name: "IX_Operations_ProductDistributionDistributionId",
                table: "Operations",
                newName: "IX_Operations_ProductDistributionId");

            migrationBuilder.InsertData(
                table: "ProductDistributions",
                columns: new[] { "ProductDistributionId", "Code", "Description", "IsActive", "ProductId" },
                values: new object[] { 1, "Dist1", "Distribution from products", true, 1 });

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_ProductDistributions_ProductDistributionId",
                table: "Operations",
                column: "ProductDistributionId",
                principalTable: "ProductDistributions",
                principalColumn: "ProductDistributionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_ProductDistributions_ProductDistributionId",
                table: "Operations");

            migrationBuilder.DeleteData(
                table: "ProductDistributions",
                keyColumn: "ProductDistributionId",
                keyValue: 1);

            migrationBuilder.RenameColumn(
                name: "ProductDistributionId",
                table: "ProductDistributions",
                newName: "DistributionId");

            migrationBuilder.RenameColumn(
                name: "ProductDistributionId",
                table: "Operations",
                newName: "ProductDistributionDistributionId");

            migrationBuilder.RenameIndex(
                name: "IX_Operations_ProductDistributionId",
                table: "Operations",
                newName: "IX_Operations_ProductDistributionDistributionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_ProductDistributions_ProductDistributionDistributionId",
                table: "Operations",
                column: "ProductDistributionDistributionId",
                principalTable: "ProductDistributions",
                principalColumn: "DistributionId");
        }
    }
}
