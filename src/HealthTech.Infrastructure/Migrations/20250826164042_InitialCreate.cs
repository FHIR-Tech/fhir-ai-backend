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
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
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
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    resource_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    fhir_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    version_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    resource_json = table.Column<string>(type: "jsonb", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    search_parameters = table.Column<string>(type: "jsonb", nullable: true),
                    tags = table.Column<string>(type: "jsonb", nullable: true),
                    security_labels = table.Column<string>(type: "jsonb", nullable: true),
                    tenant_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fhir_resources", x => x.id);
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
                name: "fhir_resources_tenant_id_resource_type_fhir_id_version_id_key",
                table: "fhir_resources",
                columns: new[] { "tenant_id", "resource_type", "fhir_id", "version_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_fhir_resources_last_updated",
                table: "fhir_resources",
                columns: new[] { "tenant_id", "last_updated" });

            migrationBuilder.CreateIndex(
                name: "idx_fhir_resources_search_params",
                table: "fhir_resources",
                column: "search_parameters")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "idx_fhir_resources_security_labels",
                table: "fhir_resources",
                column: "security_labels")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "idx_fhir_resources_tags",
                table: "fhir_resources",
                column: "tags")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "idx_fhir_resources_tenant_id",
                table: "fhir_resources",
                columns: new[] { "tenant_id", "resource_type", "fhir_id" });

            migrationBuilder.CreateIndex(
                name: "idx_fhir_resources_tenant_type",
                table: "fhir_resources",
                columns: new[] { "tenant_id", "resource_type" });
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
