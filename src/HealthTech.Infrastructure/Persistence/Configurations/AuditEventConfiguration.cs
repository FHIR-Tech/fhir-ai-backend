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

        // Configure properties
        builder.Property(e => e.Id)
            .HasColumnName("Id");

        builder.Property(e => e.TenantId)
            .HasColumnName("TenantId")
            .IsRequired();

        builder.Property(e => e.EventType)
            .HasColumnName("EventType")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.EventSubtype)
            .HasColumnName("EventSubtype")
            .HasMaxLength(100);

        builder.Property(e => e.Action)
            .HasColumnName("Action")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(e => e.Outcome)
            .HasColumnName("Outcome")
            .IsRequired();

        builder.Property(e => e.Description)
            .HasColumnName("Description")
            .HasMaxLength(1000);

        builder.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.UserDisplayName)
            .HasColumnName("UserDisplayName")
            .HasMaxLength(255);

        builder.Property(e => e.UserIpAddress)
            .HasColumnName("UserIpAddress")
            .HasMaxLength(45);

        builder.Property(e => e.ResourceType)
            .HasColumnName("ResourceType")
            .HasMaxLength(100);

        builder.Property(e => e.ResourceId)
            .HasColumnName("ResourceId")
            .HasMaxLength(255);

        builder.Property(e => e.EventData)
            .HasColumnName("EventData")
            .HasColumnType("jsonb");

        builder.Property(e => e.EventTime)
            .HasColumnName("EventTime")
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasColumnName("CreatedBy")
            .IsRequired();

        builder.Property(e => e.ModifiedAt)
            .HasColumnName("ModifiedAt");

        builder.Property(e => e.ModifiedBy)
            .HasColumnName("ModifiedBy");

        builder.Property(e => e.IsDeleted)
            .HasColumnName("IsDeleted")
            .IsRequired();

        builder.Property(e => e.Version)
            .HasColumnName("Version")
            .IsRequired();

        // Configure indexes
        builder.HasIndex(e => new { e.TenantId, e.EventTime })
            .HasDatabaseName("IX_AuditEvents_TenantId_EventTime");

        builder.HasIndex(e => new { e.TenantId, e.UserId, e.EventTime })
            .HasDatabaseName("IX_AuditEvents_TenantId_UserId_EventTime");
    }
}
