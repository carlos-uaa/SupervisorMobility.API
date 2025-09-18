using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSOSSynopticTablesSTRO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SOSAnalysisSOSDistribution",
                columns: table => new
                {
                    AnalysesSOSAnalysisId = table.Column<int>(type: "int", nullable: false),
                    DistributionsSOSDistributionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOSAnalysisSOSDistribution", x => new { x.AnalysesSOSAnalysisId, x.DistributionsSOSDistributionId });
                    table.ForeignKey(
                        name: "FK_SOSAnalysisSOSDistribution_SOSAnalyses_AnalysesSOSAnalysisId",
                        column: x => x.AnalysesSOSAnalysisId,
                        principalTable: "SOSAnalyses",
                        principalColumn: "SOSAnalysisId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SOSAnalysisSOSDistribution_SOSDistributions_DistributionsSOSDistributionId",
                        column: x => x.DistributionsSOSDistributionId,
                        principalTable: "SOSDistributions",
                        principalColumn: "SOSDistributionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SOSDistributionSOSSequence",
                columns: table => new
                {
                    DistributionsSOSDistributionId = table.Column<int>(type: "int", nullable: false),
                    SequencesSOSSequenceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOSDistributionSOSSequence", x => new { x.DistributionsSOSDistributionId, x.SequencesSOSSequenceId });
                    table.ForeignKey(
                        name: "FK_SOSDistributionSOSSequence_SOSDistributions_DistributionsSOSDistributionId",
                        column: x => x.DistributionsSOSDistributionId,
                        principalTable: "SOSDistributions",
                        principalColumn: "SOSDistributionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SOSDistributionSOSSequence_SOSSequences_SequencesSOSSequenceId",
                        column: x => x.SequencesSOSSequenceId,
                        principalTable: "SOSSequences",
                        principalColumn: "SOSSequenceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SOSSynopticTableofControlPoints",
                columns: table => new
                {
                    SOSSynopticTableofControlPointsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InternalControlNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<int>(type: "int", nullable: true),
                    ReviewerId = table.Column<int>(type: "int", nullable: true),
                    ApproverId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    SOSHubId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOSSynopticTableofControlPoints", x => x.SOSSynopticTableofControlPointsId);
                    table.ForeignKey(
                        name: "FK_SOSSynopticTableofControlPoints_Users_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_SOSSynopticTableofControlPoints_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_SOSSynopticTableofControlPoints_Users_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "SOSSynopticTableofOperatingRequirements",
                columns: table => new
                {
                    SOSSynopticTableofOperatingRequirementsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InternalControlNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<int>(type: "int", nullable: true),
                    ReviewerId = table.Column<int>(type: "int", nullable: true),
                    ApproverId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    SOSHubId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOSSynopticTableofOperatingRequirements", x => x.SOSSynopticTableofOperatingRequirementsId);
                    table.ForeignKey(
                        name: "FK_SOSSynopticTableofOperatingRequirements_Users_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_SOSSynopticTableofOperatingRequirements_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_SOSSynopticTableofOperatingRequirements_Users_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "SOSAnalysisSOSSynopticTableofControlPoints",
                columns: table => new
                {
                    AnalysesSOSAnalysisId = table.Column<int>(type: "int", nullable: false),
                    SOSSynopticControlPointsSOSSynopticTableofControlPointsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOSAnalysisSOSSynopticTableofControlPoints", x => new { x.AnalysesSOSAnalysisId, x.SOSSynopticControlPointsSOSSynopticTableofControlPointsId });
                    table.ForeignKey(
                        name: "FK_SOSAnalysisSOSSynopticTableofControlPoints_SOSAnalyses_AnalysesSOSAnalysisId",
                        column: x => x.AnalysesSOSAnalysisId,
                        principalTable: "SOSAnalyses",
                        principalColumn: "SOSAnalysisId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SOSAnalysisSOSSynopticTableofControlPoints_SOSSynopticTableofControlPoints_SOSSynopticControlPointsSOSSynopticTableofControl~",
                        column: x => x.SOSSynopticControlPointsSOSSynopticTableofControlPointsId,
                        principalTable: "SOSSynopticTableofControlPoints",
                        principalColumn: "SOSSynopticTableofControlPointsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SOSHubSOSSynopticTableofControlPoints",
                columns: table => new
                {
                    SOSHubsSOSHubId = table.Column<int>(type: "int", nullable: false),
                    SOSSynopticControlPointsSOSSynopticTableofControlPointsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOSHubSOSSynopticTableofControlPoints", x => new { x.SOSHubsSOSHubId, x.SOSSynopticControlPointsSOSSynopticTableofControlPointsId });
                    table.ForeignKey(
                        name: "FK_SOSHubSOSSynopticTableofControlPoints_SOSHubs_SOSHubsSOSHubId",
                        column: x => x.SOSHubsSOSHubId,
                        principalTable: "SOSHubs",
                        principalColumn: "SOSHubId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SOSHubSOSSynopticTableofControlPoints_SOSSynopticTableofControlPoints_SOSSynopticControlPointsSOSSynopticTableofControlPoint~",
                        column: x => x.SOSSynopticControlPointsSOSSynopticTableofControlPointsId,
                        principalTable: "SOSSynopticTableofControlPoints",
                        principalColumn: "SOSSynopticTableofControlPointsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SOSSequenceSOSSynopticTableofControlPoints",
                columns: table => new
                {
                    SOSSynopticControlPointsSOSSynopticTableofControlPointsId = table.Column<int>(type: "int", nullable: false),
                    SequencesSOSSequenceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOSSequenceSOSSynopticTableofControlPoints", x => new { x.SOSSynopticControlPointsSOSSynopticTableofControlPointsId, x.SequencesSOSSequenceId });
                    table.ForeignKey(
                        name: "FK_SOSSequenceSOSSynopticTableofControlPoints_SOSSequences_SequencesSOSSequenceId",
                        column: x => x.SequencesSOSSequenceId,
                        principalTable: "SOSSequences",
                        principalColumn: "SOSSequenceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SOSSequenceSOSSynopticTableofControlPoints_SOSSynopticTableofControlPoints_SOSSynopticControlPointsSOSSynopticTableofControl~",
                        column: x => x.SOSSynopticControlPointsSOSSynopticTableofControlPointsId,
                        principalTable: "SOSSynopticTableofControlPoints",
                        principalColumn: "SOSSynopticTableofControlPointsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SOSSynopticPointsLogbooks",
                columns: table => new
                {
                    SOSSynopticPointsLogbookId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Changes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoRevision = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    SOSSynopticTableofControlPointsId = table.Column<int>(type: "int", nullable: false),
                    ApproverId = table.Column<int>(type: "int", nullable: true),
                    ApproverSignatureImageFileUploadId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOSSynopticPointsLogbooks", x => x.SOSSynopticPointsLogbookId);
                    table.ForeignKey(
                        name: "FK_SOSSynopticPointsLogbooks_Files_ApproverSignatureImageFileUploadId",
                        column: x => x.ApproverSignatureImageFileUploadId,
                        principalTable: "Files",
                        principalColumn: "FileUploadId");
                    table.ForeignKey(
                        name: "FK_SOSSynopticPointsLogbooks_SOSSynopticTableofControlPoints_SOSSynopticTableofControlPointsId",
                        column: x => x.SOSSynopticTableofControlPointsId,
                        principalTable: "SOSSynopticTableofControlPoints",
                        principalColumn: "SOSSynopticTableofControlPointsId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SOSSynopticPointsLogbooks_Users_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "SOSSynopticPointsOperationSequences",
                columns: table => new
                {
                    SOSSynopticPointsOperationSequenceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sequence = table.Column<int>(type: "int", nullable: true),
                    SectionId = table.Column<int>(type: "int", nullable: true),
                    Times = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    SOSSynopticTableofControlPointsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOSSynopticPointsOperationSequences", x => x.SOSSynopticPointsOperationSequenceId);
                    table.ForeignKey(
                        name: "FK_SOSSynopticPointsOperationSequences_SOSSynopticTableofControlPoints_SOSSynopticTableofControlPointsId",
                        column: x => x.SOSSynopticTableofControlPointsId,
                        principalTable: "SOSSynopticTableofControlPoints",
                        principalColumn: "SOSSynopticTableofControlPointsId");
                    table.ForeignKey(
                        name: "FK_SOSSynopticPointsOperationSequences_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "SectionId");
                });

            migrationBuilder.CreateTable(
                name: "SOSAnalysisSOSSynopticTableofOperatingRequirements",
                columns: table => new
                {
                    AnalysesSOSAnalysisId = table.Column<int>(type: "int", nullable: false),
                    SOSSynopticOperatingRequirementsSOSSynopticTableofOperatingRequirementsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOSAnalysisSOSSynopticTableofOperatingRequirements", x => new { x.AnalysesSOSAnalysisId, x.SOSSynopticOperatingRequirementsSOSSynopticTableofOperatingRequirementsId });
                    table.ForeignKey(
                        name: "FK_SOSAnalysisSOSSynopticTableofOperatingRequirements_SOSAnalyses_AnalysesSOSAnalysisId",
                        column: x => x.AnalysesSOSAnalysisId,
                        principalTable: "SOSAnalyses",
                        principalColumn: "SOSAnalysisId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SOSAnalysisSOSSynopticTableofOperatingRequirements_SOSSynopticTableofOperatingRequirements_SOSSynopticOperatingRequirementsS~",
                        column: x => x.SOSSynopticOperatingRequirementsSOSSynopticTableofOperatingRequirementsId,
                        principalTable: "SOSSynopticTableofOperatingRequirements",
                        principalColumn: "SOSSynopticTableofOperatingRequirementsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SOSHubSOSSynopticTableofOperatingRequirements",
                columns: table => new
                {
                    SOSHubsSOSHubId = table.Column<int>(type: "int", nullable: false),
                    SOSSynopticOperatingRequirementsSOSSynopticTableofOperatingRequirementsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOSHubSOSSynopticTableofOperatingRequirements", x => new { x.SOSHubsSOSHubId, x.SOSSynopticOperatingRequirementsSOSSynopticTableofOperatingRequirementsId });
                    table.ForeignKey(
                        name: "FK_SOSHubSOSSynopticTableofOperatingRequirements_SOSHubs_SOSHubsSOSHubId",
                        column: x => x.SOSHubsSOSHubId,
                        principalTable: "SOSHubs",
                        principalColumn: "SOSHubId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SOSHubSOSSynopticTableofOperatingRequirements_SOSSynopticTableofOperatingRequirements_SOSSynopticOperatingRequirementsSOSSyn~",
                        column: x => x.SOSSynopticOperatingRequirementsSOSSynopticTableofOperatingRequirementsId,
                        principalTable: "SOSSynopticTableofOperatingRequirements",
                        principalColumn: "SOSSynopticTableofOperatingRequirementsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SOSSequenceSOSSynopticTableofOperatingRequirements",
                columns: table => new
                {
                    SOSSynopticOperatingRequirementsSOSSynopticTableofOperatingRequirementsId = table.Column<int>(type: "int", nullable: false),
                    SequencesSOSSequenceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOSSequenceSOSSynopticTableofOperatingRequirements", x => new { x.SOSSynopticOperatingRequirementsSOSSynopticTableofOperatingRequirementsId, x.SequencesSOSSequenceId });
                    table.ForeignKey(
                        name: "FK_SOSSequenceSOSSynopticTableofOperatingRequirements_SOSSequences_SequencesSOSSequenceId",
                        column: x => x.SequencesSOSSequenceId,
                        principalTable: "SOSSequences",
                        principalColumn: "SOSSequenceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SOSSequenceSOSSynopticTableofOperatingRequirements_SOSSynopticTableofOperatingRequirements_SOSSynopticOperatingRequirementsS~",
                        column: x => x.SOSSynopticOperatingRequirementsSOSSynopticTableofOperatingRequirementsId,
                        principalTable: "SOSSynopticTableofOperatingRequirements",
                        principalColumn: "SOSSynopticTableofOperatingRequirementsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SOSSynopticRequirementsLogbooks",
                columns: table => new
                {
                    SOSSynopticRequirementsLogbookId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Changes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoRevision = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    SOSSynopticRequirementsId = table.Column<int>(type: "int", nullable: false),
                    ApproverId = table.Column<int>(type: "int", nullable: true),
                    ApproverSignatureImageFileUploadId = table.Column<int>(type: "int", nullable: true),
                    ReviewerId = table.Column<int>(type: "int", nullable: true),
                    ReviewerSignatureImageFileUploadId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOSSynopticRequirementsLogbooks", x => x.SOSSynopticRequirementsLogbookId);
                    table.ForeignKey(
                        name: "FK_SOSSynopticRequirementsLogbooks_Files_ApproverSignatureImageFileUploadId",
                        column: x => x.ApproverSignatureImageFileUploadId,
                        principalTable: "Files",
                        principalColumn: "FileUploadId");
                    table.ForeignKey(
                        name: "FK_SOSSynopticRequirementsLogbooks_Files_ReviewerSignatureImageFileUploadId",
                        column: x => x.ReviewerSignatureImageFileUploadId,
                        principalTable: "Files",
                        principalColumn: "FileUploadId");
                    table.ForeignKey(
                        name: "FK_SOSSynopticRequirementsLogbooks_SOSSynopticTableofOperatingRequirements_SOSSynopticRequirementsId",
                        column: x => x.SOSSynopticRequirementsId,
                        principalTable: "SOSSynopticTableofOperatingRequirements",
                        principalColumn: "SOSSynopticTableofOperatingRequirementsId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SOSSynopticRequirementsLogbooks_Users_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_SOSSynopticRequirementsLogbooks_Users_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "SOSSynopticRequirementsOperationSequences",
                columns: table => new
                {
                    SOSSynopticRequirementsOperationSequenceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sequence = table.Column<int>(type: "int", nullable: true),
                    SectionId = table.Column<int>(type: "int", nullable: true),
                    Times = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    SOSSynopticTableofOperatingRequirementsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOSSynopticRequirementsOperationSequences", x => x.SOSSynopticRequirementsOperationSequenceId);
                    table.ForeignKey(
                        name: "FK_SOSSynopticRequirementsOperationSequences_SOSSynopticTableofOperatingRequirements_SOSSynopticTableofOperatingRequirementsId",
                        column: x => x.SOSSynopticTableofOperatingRequirementsId,
                        principalTable: "SOSSynopticTableofOperatingRequirements",
                        principalColumn: "SOSSynopticTableofOperatingRequirementsId");
                    table.ForeignKey(
                        name: "FK_SOSSynopticRequirementsOperationSequences_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "SectionId");
                });

            migrationBuilder.CreateTable(
                name: "SOSSynopticTableRequirementOperationDifficulty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SOSSynopticTableofOperatingRequirementsId = table.Column<int>(type: "int", nullable: false),
                    SOSHubId = table.Column<int>(type: "int", nullable: false),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOSSynopticTableRequirementOperationDifficulty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SOSSynopticTableRequirementOperationDifficulty_SOSHubs_SOSHubId",
                        column: x => x.SOSHubId,
                        principalTable: "SOSHubs",
                        principalColumn: "SOSHubId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SOSSynopticTableRequirementOperationDifficulty_SOSSynopticTableofOperatingRequirements_SOSSynopticTableofOperatingRequiremen~",
                        column: x => x.SOSSynopticTableofOperatingRequirementsId,
                        principalTable: "SOSSynopticTableofOperatingRequirements",
                        principalColumn: "SOSSynopticTableofOperatingRequirementsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SOSAnalysisSOSDistribution_DistributionsSOSDistributionId",
                table: "SOSAnalysisSOSDistribution",
                column: "DistributionsSOSDistributionId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSAnalysisSOSSynopticTableofControlPoints_SOSSynopticControlPointsSOSSynopticTableofControlPointsId",
                table: "SOSAnalysisSOSSynopticTableofControlPoints",
                column: "SOSSynopticControlPointsSOSSynopticTableofControlPointsId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSAnalysisSOSSynopticTableofOperatingRequirements_SOSSynopticOperatingRequirementsSOSSynopticTableofOperatingRequirementsId",
                table: "SOSAnalysisSOSSynopticTableofOperatingRequirements",
                column: "SOSSynopticOperatingRequirementsSOSSynopticTableofOperatingRequirementsId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSDistributionSOSSequence_SequencesSOSSequenceId",
                table: "SOSDistributionSOSSequence",
                column: "SequencesSOSSequenceId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSHubSOSSynopticTableofControlPoints_SOSSynopticControlPointsSOSSynopticTableofControlPointsId",
                table: "SOSHubSOSSynopticTableofControlPoints",
                column: "SOSSynopticControlPointsSOSSynopticTableofControlPointsId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSHubSOSSynopticTableofOperatingRequirements_SOSSynopticOperatingRequirementsSOSSynopticTableofOperatingRequirementsId",
                table: "SOSHubSOSSynopticTableofOperatingRequirements",
                column: "SOSSynopticOperatingRequirementsSOSSynopticTableofOperatingRequirementsId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSequenceSOSSynopticTableofControlPoints_SequencesSOSSequenceId",
                table: "SOSSequenceSOSSynopticTableofControlPoints",
                column: "SequencesSOSSequenceId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSequenceSOSSynopticTableofOperatingRequirements_SequencesSOSSequenceId",
                table: "SOSSequenceSOSSynopticTableofOperatingRequirements",
                column: "SequencesSOSSequenceId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSynopticPointsLogbooks_ApproverId",
                table: "SOSSynopticPointsLogbooks",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSynopticPointsLogbooks_ApproverSignatureImageFileUploadId",
                table: "SOSSynopticPointsLogbooks",
                column: "ApproverSignatureImageFileUploadId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSynopticPointsLogbooks_SOSSynopticTableofControlPointsId",
                table: "SOSSynopticPointsLogbooks",
                column: "SOSSynopticTableofControlPointsId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSynopticPointsOperationSequences_SectionId",
                table: "SOSSynopticPointsOperationSequences",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSynopticPointsOperationSequences_SOSSynopticTableofControlPointsId",
                table: "SOSSynopticPointsOperationSequences",
                column: "SOSSynopticTableofControlPointsId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSynopticRequirementsLogbooks_ApproverId",
                table: "SOSSynopticRequirementsLogbooks",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSynopticRequirementsLogbooks_ApproverSignatureImageFileUploadId",
                table: "SOSSynopticRequirementsLogbooks",
                column: "ApproverSignatureImageFileUploadId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSynopticRequirementsLogbooks_ReviewerId",
                table: "SOSSynopticRequirementsLogbooks",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSynopticRequirementsLogbooks_ReviewerSignatureImageFileUploadId",
                table: "SOSSynopticRequirementsLogbooks",
                column: "ReviewerSignatureImageFileUploadId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSynopticRequirementsLogbooks_SOSSynopticRequirementsId",
                table: "SOSSynopticRequirementsLogbooks",
                column: "SOSSynopticRequirementsId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSynopticRequirementsOperationSequences_SectionId",
                table: "SOSSynopticRequirementsOperationSequences",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSynopticRequirementsOperationSequences_SOSSynopticTableofOperatingRequirementsId",
                table: "SOSSynopticRequirementsOperationSequences",
                column: "SOSSynopticTableofOperatingRequirementsId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSynopticTableofControlPoints_ApproverId",
                table: "SOSSynopticTableofControlPoints",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSynopticTableofControlPoints_CreatorId",
                table: "SOSSynopticTableofControlPoints",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSynopticTableofControlPoints_ReviewerId",
                table: "SOSSynopticTableofControlPoints",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSynopticTableofOperatingRequirements_ApproverId",
                table: "SOSSynopticTableofOperatingRequirements",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSynopticTableofOperatingRequirements_CreatorId",
                table: "SOSSynopticTableofOperatingRequirements",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSynopticTableofOperatingRequirements_ReviewerId",
                table: "SOSSynopticTableofOperatingRequirements",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSynopticTableRequirementOperationDifficulty_SOSHubId",
                table: "SOSSynopticTableRequirementOperationDifficulty",
                column: "SOSHubId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSSynopticTableRequirementOperationDifficulty_SOSSynopticTableofOperatingRequirementsId",
                table: "SOSSynopticTableRequirementOperationDifficulty",
                column: "SOSSynopticTableofOperatingRequirementsId");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HCIs_SOSHubs_SOSHubId",
                table: "HCIs");

            migrationBuilder.DropForeignKey(
                name: "FK_SOSAnalyses_SOSHubs_SOSHubId",
                table: "SOSAnalyses");

            migrationBuilder.DropForeignKey(
                name: "FK_SOSCombinations_SOSHubs_SOSHubId",
                table: "SOSCombinations");

            migrationBuilder.DropForeignKey(
                name: "FK_SOSSequences_SOSHubs_SOSHubId",
                table: "SOSSequences");

            migrationBuilder.DropTable(
                name: "SOSAnalysisSOSDistribution");

            migrationBuilder.DropTable(
                name: "SOSAnalysisSOSSynopticTableofControlPoints");

            migrationBuilder.DropTable(
                name: "SOSAnalysisSOSSynopticTableofOperatingRequirements");

            migrationBuilder.DropTable(
                name: "SOSDistributionSOSSequence");

            migrationBuilder.DropTable(
                name: "SOSHubSOSSynopticTableofControlPoints");

            migrationBuilder.DropTable(
                name: "SOSHubSOSSynopticTableofOperatingRequirements");

            migrationBuilder.DropTable(
                name: "SOSSequenceSOSSynopticTableofControlPoints");

            migrationBuilder.DropTable(
                name: "SOSSequenceSOSSynopticTableofOperatingRequirements");

            migrationBuilder.DropTable(
                name: "SOSSynopticPointsLogbooks");

            migrationBuilder.DropTable(
                name: "SOSSynopticPointsOperationSequences");

            migrationBuilder.DropTable(
                name: "SOSSynopticRequirementsLogbooks");

            migrationBuilder.DropTable(
                name: "SOSSynopticRequirementsOperationSequences");

            migrationBuilder.DropTable(
                name: "SOSSynopticTableRequirementOperationDifficulty");

            migrationBuilder.DropTable(
                name: "SOSSynopticTableofControlPoints");

            migrationBuilder.DropTable(
                name: "SOSSynopticTableofOperatingRequirements");
        }
    }
}
