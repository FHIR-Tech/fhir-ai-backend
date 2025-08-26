using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTech.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "audit_events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EventSubtype = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Action = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Outcome = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    UserId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UserDisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UserIpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    ResourceType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ResourceId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EventData = table.Column<string>(type: "jsonb", nullable: true),
                    EventTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_events", x => x.Id);
                },
                comment: "Audit events for compliance tracking");

            migrationBuilder.CreateTable(
                name: "fhir_resources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FhirId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    VersionId = table.Column<int>(type: "integer", nullable: false),
                    ResourceJson = table.Column<string>(type: "jsonb", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SearchParameters = table.Column<string>(type: "jsonb", nullable: true),
                    Tags = table.Column<string>(type: "jsonb", nullable: true),
                    SecurityLabels = table.Column<string>(type: "jsonb", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fhir_resources", x => x.Id);
                },
                comment: "FHIR resources stored as JSONB");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_TenantId_EventTime",
                table: "audit_events",
                columns: new[] { "TenantId", "EventTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_TenantId_UserId_EventTime",
                table: "audit_events",
                columns: new[] { "TenantId", "UserId", "EventTime" });

            migrationBuilder.CreateIndex(
                name: "IX_fhir_resources_TenantId_ResourceType_FhirId",
                table: "fhir_resources",
                columns: new[] { "TenantId", "ResourceType", "FhirId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FhirResources_SearchParameters",
                table: "fhir_resources",
                column: "SearchParameters")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "IX_FhirResources_TenantId_ResourceType",
                table: "fhir_resources",
                columns: new[] { "TenantId", "ResourceType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_events");

            migrationBuilder.DropTable(
                name: "fhir_resources");
        }
    }
}
