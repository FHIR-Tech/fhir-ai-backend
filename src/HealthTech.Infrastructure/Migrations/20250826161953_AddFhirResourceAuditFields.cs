using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTech.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFhirResourceAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_fhir_resources_TenantId_ResourceType_FhirId",
                table: "fhir_resources");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "fhir_resources",
                newName: "version");

            migrationBuilder.RenameColumn(
                name: "Tags",
                table: "fhir_resources",
                newName: "tags");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "fhir_resources",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "fhir_resources",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "VersionId",
                table: "fhir_resources",
                newName: "version_id");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "fhir_resources",
                newName: "tenant_id");

            migrationBuilder.RenameColumn(
                name: "SecurityLabels",
                table: "fhir_resources",
                newName: "security_labels");

            migrationBuilder.RenameColumn(
                name: "SearchParameters",
                table: "fhir_resources",
                newName: "search_parameters");

            migrationBuilder.RenameColumn(
                name: "ResourceType",
                table: "fhir_resources",
                newName: "resource_type");

            migrationBuilder.RenameColumn(
                name: "ResourceJson",
                table: "fhir_resources",
                newName: "resource_json");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "fhir_resources",
                newName: "modified_by");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                table: "fhir_resources",
                newName: "modified_at");

            migrationBuilder.RenameColumn(
                name: "LastUpdated",
                table: "fhir_resources",
                newName: "last_updated");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "fhir_resources",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "FhirId",
                table: "fhir_resources",
                newName: "fhir_id");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "fhir_resources",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "fhir_resources",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_FhirResources_TenantId_ResourceType",
                table: "fhir_resources",
                newName: "idx_fhir_resources_tenant_type");

            migrationBuilder.RenameIndex(
                name: "IX_FhirResources_SearchParameters",
                table: "fhir_resources",
                newName: "idx_fhir_resources_search_params");

            migrationBuilder.AlterColumn<int>(
                name: "version",
                table: "fhir_resources",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "fhir_resources",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "version_id",
                table: "fhir_resources",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "tenant_id",
                table: "fhir_resources",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "modified_by",
                table: "fhir_resources",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_deleted",
                table: "fhir_resources",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "fhir_resources",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "fhir_resources",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "fhir_resources",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "fhir_resources",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "fhir_resources_tenant_id_resource_type_fhir_id_version_id_key",
                table: "fhir_resources");

            migrationBuilder.DropIndex(
                name: "idx_fhir_resources_last_updated",
                table: "fhir_resources");

            migrationBuilder.DropIndex(
                name: "idx_fhir_resources_security_labels",
                table: "fhir_resources");

            migrationBuilder.DropIndex(
                name: "idx_fhir_resources_tags",
                table: "fhir_resources");

            migrationBuilder.DropIndex(
                name: "idx_fhir_resources_tenant_id",
                table: "fhir_resources");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "fhir_resources");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "fhir_resources");

            migrationBuilder.RenameColumn(
                name: "version",
                table: "fhir_resources",
                newName: "Version");

            migrationBuilder.RenameColumn(
                name: "tags",
                table: "fhir_resources",
                newName: "Tags");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "fhir_resources",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "fhir_resources",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "version_id",
                table: "fhir_resources",
                newName: "VersionId");

            migrationBuilder.RenameColumn(
                name: "tenant_id",
                table: "fhir_resources",
                newName: "TenantId");

            migrationBuilder.RenameColumn(
                name: "security_labels",
                table: "fhir_resources",
                newName: "SecurityLabels");

            migrationBuilder.RenameColumn(
                name: "search_parameters",
                table: "fhir_resources",
                newName: "SearchParameters");

            migrationBuilder.RenameColumn(
                name: "resource_type",
                table: "fhir_resources",
                newName: "ResourceType");

            migrationBuilder.RenameColumn(
                name: "resource_json",
                table: "fhir_resources",
                newName: "ResourceJson");

            migrationBuilder.RenameColumn(
                name: "modified_by",
                table: "fhir_resources",
                newName: "ModifiedBy");

            migrationBuilder.RenameColumn(
                name: "modified_at",
                table: "fhir_resources",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "last_updated",
                table: "fhir_resources",
                newName: "LastUpdated");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "fhir_resources",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "fhir_id",
                table: "fhir_resources",
                newName: "FhirId");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "fhir_resources",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "fhir_resources",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "idx_fhir_resources_tenant_type",
                table: "fhir_resources",
                newName: "IX_FhirResources_TenantId_ResourceType");

            migrationBuilder.RenameIndex(
                name: "idx_fhir_resources_search_params",
                table: "fhir_resources",
                newName: "IX_FhirResources_SearchParameters");

            migrationBuilder.AlterColumn<int>(
                name: "Version",
                table: "fhir_resources",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "fhir_resources",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AlterColumn<int>(
                name: "VersionId",
                table: "fhir_resources",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "fhir_resources",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "fhir_resources",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "fhir_resources",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "fhir_resources",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "fhir_resources",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.CreateIndex(
                name: "IX_fhir_resources_TenantId_ResourceType_FhirId",
                table: "fhir_resources",
                columns: new[] { "TenantId", "ResourceType", "FhirId" },
                unique: true);
        }
    }
}
