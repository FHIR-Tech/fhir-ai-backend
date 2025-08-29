using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HealthTech.Domain.Entities;

namespace HealthTech.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for AuditEvent
/// </summary>
public class AuditEventConfiguration : IEntityTypeConfiguration<AuditEvent>
{
    /// <summary>
    /// Configure the entity
    /// </summary>
    /// <param name="builder">Entity type builder</param>
    public void Configure(EntityTypeBuilder<AuditEvent> builder)
    {
        builder.ToTable("audit_events", t => t.HasComment("Audit events for compliance tracking"));

        // Configure primary key
        builder.HasKey(e => e.Id);

        // Configure properties with explicit column names for consistency
        builder.Property(e => e.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(e => e.TenantId)
            .HasColumnName("tenant_id")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.EventType)
            .HasColumnName("event_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.EventSubtype)
            .HasColumnName("event_subtype")
            .HasMaxLength(100);

        builder.Property(e => e.Action)
            .HasColumnName("action")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(e => e.Outcome)
            .HasColumnName("outcome")
            .IsRequired();

        builder.Property(e => e.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        builder.Property(e => e.UserId)
            .HasColumnName("user_id")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.UserDisplayName)
            .HasColumnName("user_display_name")
            .HasMaxLength(255);

        builder.Property(e => e.UserIpAddress)
            .HasColumnName("user_ip_address")
            .HasMaxLength(45);

        builder.Property(e => e.ResourceType)
            .HasColumnName("resource_type")
            .HasMaxLength(100);

        builder.Property(e => e.ResourceId)
            .HasColumnName("resource_id")
            .HasMaxLength(255);

        builder.Property(e => e.EventData)
            .HasColumnName("event_data")
            .HasColumnType("jsonb");

        builder.Property(e => e.EventTime)
            .HasColumnName("event_time")
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

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
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(e => e.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(e => e.DeletedBy)
            .HasColumnName("deleted_by")
            .HasMaxLength(255);

        builder.Property(e => e.Version)
            .HasColumnName("version")
            .HasDefaultValue(1)
            .IsRequired();

        // Configure indexes
        builder.HasIndex(e => new { e.TenantId, e.EventTime });

        builder.HasIndex(e => new { e.TenantId, e.UserId, e.EventTime });
    }
}
