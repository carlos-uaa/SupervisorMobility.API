using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class First : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChecklistCategories",
                columns: table => new
                {
                    ChecklistCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistCategories", x => x.ChecklistCategoryId);
                    table.CheckConstraint("ck_cc_seq", "[Sequence] > 0");
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.GroupId);
                });

            migrationBuilder.CreateTable(
                name: "JobObservationTypes",
                columns: table => new
                {
                    JobObservationTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobObservationTypes", x => x.JobObservationTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Plants",
                columns: table => new
                {
                    PlantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plants", x => x.PlantId);
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
                name: "QuestionTypes",
                columns: table => new
                {
                    QuestionTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionTypes", x => x.QuestionTypeId);
                });

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

            migrationBuilder.CreateTable(
                name: "JobObservationConfigs",
                columns: table => new
                {
                    JobObservationConfigId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobObservationTypeId = table.Column<int>(type: "int", nullable: false),
                    ChecklistCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobObservationConfigs", x => x.JobObservationConfigId);
                    table.ForeignKey(
                        name: "FK_JobObservationConfigs_ChecklistCategories_ChecklistCategoryId",
                        column: x => x.ChecklistCategoryId,
                        principalTable: "ChecklistCategories",
                        principalColumn: "ChecklistCategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobObservationConfigs_JobObservationTypes_JobObservationTypeId",
                        column: x => x.JobObservationTypeId,
                        principalTable: "JobObservationTypes",
                        principalColumn: "JobObservationTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    AreaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    PlantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.AreaId);
                    table.ForeignKey(
                        name: "FK_Areas_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "PlantId",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "ChecklistQuestions",
                columns: table => new
                {
                    QuestionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Prompt = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CategorySequence = table.Column<int>(type: "int", nullable: false),
                    AnswerSetID = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    ChecklistCategoryId = table.Column<int>(type: "int", nullable: false),
                    QuestionTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistQuestions", x => x.QuestionID);
                    table.CheckConstraint("ck_cq_seq", "[CategorySequence] > 0");
                    table.ForeignKey(
                        name: "FK_ChecklistQuestions_ChecklistCategories_ChecklistCategoryId",
                        column: x => x.ChecklistCategoryId,
                        principalTable: "ChecklistCategories",
                        principalColumn: "ChecklistCategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChecklistQuestions_QuestionTypes_QuestionTypeId",
                        column: x => x.QuestionTypeId,
                        principalTable: "QuestionTypes",
                        principalColumn: "QuestionTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Distributions",
                columns: table => new
                {
                    DistributionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    AreaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distributions", x => x.DistributionId);
                    table.ForeignKey(
                        name: "FK_Distributions_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Operations",
                columns: table => new
                {
                    OperationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    DistributionId = table.Column<int>(type: "int", nullable: false),
                    ProductDistributionId = table.Column<int>(type: "int", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_Operations_ProductDistributions_ProductDistributionId",
                        column: x => x.ProductDistributionId,
                        principalTable: "ProductDistributions",
                        principalColumn: "ProductDistributionId");
                });

            migrationBuilder.CreateTable(
                name: "AssyCharts",
                columns: table => new
                {
                    AssyChardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
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
                table: "ChecklistCategories",
                columns: new[] { "ChecklistCategoryId", "Code", "Description", "IsActive", "Sequence" },
                values: new object[,]
                {
                    { 1, "PO", "Preparación de la Observación", true, 1 },
                    { 2, "OPCE", "Observación para el cumplimiento del estándar - Observación de lejos", true, 2 },
                    { 3, "ATO", "Análisis de tiempo de operación", true, 3 },
                    { 4, "OCE", "Observación para cumplimiento del estándar - Observación de cerca", true, 4 },
                    { 5, "OMEFE", "Observación para mejora del estándar de acuerdo al filtro elegido", true, 5 },
                    { 6, "TOSF", "Trabajo de Observación  - Sumario / Finalización", true, 6 }
                });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "GroupId", "Code", "Description", "IsActive" },
                values: new object[,]
                {
                    { 1, "GA", "Grupo A", true },
                    { 2, "GB", "Grupo B", true }
                });

            migrationBuilder.InsertData(
                table: "JobObservationTypes",
                columns: new[] { "JobObservationTypeId", "Code", "Description", "IsActive" },
                values: new object[,]
                {
                    { 1, "JC", "Observación de Operación Cíclica", true },
                    { 2, "JNC", "Observación de Operación No Cíclica", true }
                });

            migrationBuilder.InsertData(
                table: "Plants",
                columns: new[] { "PlantId", "Code", "Description", "IsActive" },
                values: new object[,]
                {
                    { 1, "T&C", "Trim and Chassis", true },
                    { 2, "Paint", "Paint", true }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Code", "Description", "IsActive" },
                values: new object[,]
                {
                    { 1, "P71A", "Infiniti P71A", true },
                    { 3, "X247", "Mercedes X247", true }
                });

            migrationBuilder.InsertData(
                table: "QuestionTypes",
                columns: new[] { "QuestionTypeId", "Code", "Description", "IsActive" },
                values: new object[,]
                {
                    { 1, "TXT", "Free text", true },
                    { 2, "MC", "Multiple Choice", true },
                    { 3, "NMB", "Number", true },
                    { 4, "Date", "Date", true },
                    { 5, "TM", "Time", true },
                    { 6, "TF", "Si/No", true }
                });

            migrationBuilder.InsertData(
                table: "SupportDocumentTypes",
                columns: new[] { "SupportDocumentTypeId", "Code", "Description", "IsActive" },
                values: new object[,]
                {
                    { 1, "GOS", "GOS", true },
                    { 2, "HOE", "HOE", true }
                });

            migrationBuilder.InsertData(
                table: "Areas",
                columns: new[] { "AreaId", "Code", "Description", "IsActive", "PlantId" },
                values: new object[] { 1, "T1", "Trim 1", true, 1 });

            migrationBuilder.InsertData(
                table: "ChecklistQuestions",
                columns: new[] { "QuestionID", "AnswerSetID", "CategorySequence", "ChecklistCategoryId", "Code", "Description", "IsActive", "Prompt", "QuestionTypeId" },
                values: new object[,]
                {
                    { 1, null, 1, 1, "PO:ECA", "Estandares completos y actualizados", true, "Los estándares estan completos y actualizados (HOE, Estado de referencia de 5S, etc. Icluyendo la pasada observación de operación  (S/N)", 6 },
                    { 2, null, 2, 1, "PO:NIO", "Nivel ILU del operador", true, "¿Cuál es nivel de ILU del operador?  ¿Está el entrenamiento alineado con el Cuadro de requisitos de Operaicón ? (S/N)", 6 }
                });

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

            migrationBuilder.InsertData(
                table: "ProductDistributions",
                columns: new[] { "ProductDistributionId", "Code", "Description", "IsActive", "ProductId" },
                values: new object[] { 1, "Dist1", "Distribution from products", true, 1 });

            migrationBuilder.InsertData(
                table: "Distributions",
                columns: new[] { "DistributionId", "AreaId", "Code", "Description", "IsActive" },
                values: new object[] { 1, 1, "Dist1", "Distribution 1 Trim 1", true });

            migrationBuilder.InsertData(
                table: "AssyCharts",
                columns: new[] { "AssyChardId", "AreaId", "CCP", "CreationDate", "DistributionId", "GOS", "HOE", "IsActive", "ModificationDate", "OperationId", "PlantId", "ProductId" },
                values: new object[] { 1, 1, "string", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "string", "string", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 1 });

            migrationBuilder.InsertData(
                table: "Operations",
                columns: new[] { "OperationId", "Code", "Description", "DistributionId", "IsActive", "ProductDistributionId" },
                values: new object[] { 1, "OP1", "Operacion Trim 1", 1, true, null });

            migrationBuilder.CreateIndex(
                name: "IX_Areas_PlantId",
                table: "Areas",
                column: "PlantId");

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
                name: "ix_cc_cod",
                table: "ChecklistCategories",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistQuestions_ChecklistCategoryId",
                table: "ChecklistQuestions",
                column: "ChecklistCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistQuestions_QuestionTypeId",
                table: "ChecklistQuestions",
                column: "QuestionTypeId");

            migrationBuilder.CreateIndex(
                name: "ix_cq_cod",
                table: "ChecklistQuestions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Distributions_AreaId",
                table: "Distributions",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_JobObservationConfigs_ChecklistCategoryId",
                table: "JobObservationConfigs",
                column: "ChecklistCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_JobObservationConfigs_JobObservationTypeId",
                table: "JobObservationConfigs",
                column: "JobObservationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_DistributionId",
                table: "Operations",
                column: "DistributionId");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_ProductDistributionId",
                table: "Operations",
                column: "ProductDistributionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDistributions_ProductId",
                table: "ProductDistributions",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssyCharts");

            migrationBuilder.DropTable(
                name: "ChecklistQuestions");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "JobObservationConfigs");

            migrationBuilder.DropTable(
                name: "SupportDocumentTypes");

            migrationBuilder.DropTable(
                name: "Operations");

            migrationBuilder.DropTable(
                name: "QuestionTypes");

            migrationBuilder.DropTable(
                name: "ChecklistCategories");

            migrationBuilder.DropTable(
                name: "JobObservationTypes");

            migrationBuilder.DropTable(
                name: "Distributions");

            migrationBuilder.DropTable(
                name: "ProductDistributions");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Plants");
        }
    }
}
