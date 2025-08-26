using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HealthTech.Domain.Entities;

namespace HealthTech.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for FhirResource
/// </summary>
public class FhirResourceConfiguration : IEntityTypeConfiguration<FhirResource>
{
    /// <summary>
    /// Configure the entity
    /// </summary>
    /// <param name="builder">Entity type builder</param>
    public void Configure(EntityTypeBuilder<FhirResource> builder)
    {
        builder.ToTable("fhir_resources", t => t.HasComment("FHIR resources stored as JSONB"));

        // Configure primary key
        builder.HasKey(e => e.Id);

        // Configure properties
        builder.Property(e => e.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(e => e.TenantId)
            .HasColumnName("tenant_id")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.ResourceType)
            .HasColumnName("resource_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.FhirId)
            .HasColumnName("fhir_id")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.VersionId)
            .HasColumnName("version_id")
            .HasDefaultValue(1);

        builder.Property(e => e.ResourceJson)
            .HasColumnName("resource_json")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(e => e.Status)
            .HasColumnName("status")
            .HasMaxLength(50);

        builder.Property(e => e.LastUpdated)
            .HasColumnName("last_updated");

        builder.Property(e => e.SearchParameters)
            .HasColumnName("search_parameters")
            .HasColumnType("jsonb");

        builder.Property(e => e.Tags)
            .HasColumnName("tags")
            .HasColumnType("jsonb");

        builder.Property(e => e.SecurityLabels)
            .HasColumnName("security_labels")
            .HasColumnType("jsonb");

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.ModifiedAt)
            .HasColumnName("modified_at");

        builder.Property(e => e.ModifiedBy)
            .HasColumnName("modified_by")
            .HasMaxLength(255);

        builder.Property(e => e.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false);

        builder.Property(e => e.Version)
            .HasColumnName("version")
            .HasDefaultValue(1);

        // Configure indexes
        builder.HasIndex(e => new { e.TenantId, e.ResourceType, e.FhirId, e.VersionId })
            .IsUnique()
            .HasDatabaseName("fhir_resources_tenant_id_resource_type_fhir_id_version_id_key");

        builder.HasIndex(e => new { e.TenantId, e.LastUpdated })
            .HasDatabaseName("idx_fhir_resources_last_updated");

        builder.HasIndex(e => e.SearchParameters)
            .HasMethod("gin")
            .HasDatabaseName("idx_fhir_resources_search_params");

        builder.HasIndex(e => e.Tags)
            .HasMethod("gin")
            .HasDatabaseName("idx_fhir_resources_tags");

        builder.HasIndex(e => e.SecurityLabels)
            .HasMethod("gin")
            .HasDatabaseName("idx_fhir_resources_security_labels");

        builder.HasIndex(e => new { e.TenantId, e.ResourceType, e.FhirId })
            .HasDatabaseName("idx_fhir_resources_tenant_id");

        builder.HasIndex(e => new { e.TenantId, e.ResourceType })
            .HasDatabaseName("idx_fhir_resources_tenant_type");
    }
}
