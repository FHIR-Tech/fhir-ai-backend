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

        // Configure properties - Remove explicit column names to use snake_case convention
        builder.Property(e => e.TenantId)
            .IsRequired();

        builder.Property(e => e.EventType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.EventSubtype)
            .HasMaxLength(100);

        builder.Property(e => e.Action)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(e => e.Outcome)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(1000);

        builder.Property(e => e.UserId)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.UserDisplayName)
            .HasMaxLength(255);

        builder.Property(e => e.UserIpAddress)
            .HasMaxLength(45);

        builder.Property(e => e.ResourceType)
            .HasMaxLength(100);

        builder.Property(e => e.ResourceId)
            .HasMaxLength(255);

        builder.Property(e => e.EventData)
            .HasColumnType("jsonb");

        builder.Property(e => e.EventTime)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .IsRequired();

        builder.Property(e => e.ModifiedAt);

        builder.Property(e => e.ModifiedBy);

        builder.Property(e => e.IsDeleted)
            .IsRequired();

        builder.Property(e => e.Version)
            .IsRequired();

        // Configure indexes
        builder.HasIndex(e => new { e.TenantId, e.EventTime });

        builder.HasIndex(e => new { e.TenantId, e.UserId, e.EventTime });
    }
}
