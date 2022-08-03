using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    public partial class AddSeedingForJobObservationConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "JobObservationTypes",
                columns: new[] { "JobObservationTypeId", "Code", "Description", "IsActive" },
                values: new object[] { 1, "JC", "Observación de Operación Cíclica", true });

            migrationBuilder.InsertData(
                table: "JobObservationTypes",
                columns: new[] { "JobObservationTypeId", "Code", "Description", "IsActive" },
                values: new object[] { 2, "JNC", "Observación de Operación No Cíclica", true });

            migrationBuilder.InsertData(
                table: "JobObservationConfigs",
                columns: new[] { "JobObservationConfigId", "ChecklistCategoryId", "JobObservationTypeId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 2, 1 },
                    { 3, 3, 1 },
                    { 4, 4, 1 },
                    { 5, 5, 1 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "JobObservationConfigs",
                keyColumn: "JobObservationConfigId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "JobObservationConfigs",
                keyColumn: "JobObservationConfigId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "JobObservationConfigs",
                keyColumn: "JobObservationConfigId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "JobObservationConfigs",
                keyColumn: "JobObservationConfigId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "JobObservationConfigs",
                keyColumn: "JobObservationConfigId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "JobObservationTypes",
                keyColumn: "JobObservationTypeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "JobObservationTypes",
                keyColumn: "JobObservationTypeId",
                keyValue: 1);
        }
    }
}
