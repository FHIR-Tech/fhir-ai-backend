using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HealthTech.Domain.Entities;
using HealthTech.Domain.Enums;

namespace HealthTech.Infrastructure.Persistence.Configurations;

public class PatientAccessConfiguration : IEntityTypeConfiguration<PatientAccess>
{
    public void Configure(EntityTypeBuilder<PatientAccess> builder)
    {
        builder.ToTable("patient_accesses");

        builder.HasKey(pa => pa.Id);
        builder.Property(pa => pa.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(pa => pa.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(pa => pa.PatientId).HasColumnName("patient_id").IsRequired();
        builder.Property(pa => pa.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(pa => pa.AccessLevel).HasColumnName("access_level").HasConversion<string>().IsRequired();
        builder.Property(pa => pa.GrantedAt).HasColumnName("granted_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(pa => pa.ExpiresAt).HasColumnName("expires_at");
        builder.Property(pa => pa.IsEmergencyAccess).HasColumnName("is_emergency_access").HasDefaultValue(false);
        builder.Property(pa => pa.EmergencyJustification).HasColumnName("emergency_justification");
        builder.Property(pa => pa.GrantedBy).HasColumnName("granted_by").IsRequired();
        builder.Property(pa => pa.Reason).HasColumnName("reason");
        builder.Property(pa => pa.IsEnabled).HasColumnName("is_enabled").HasDefaultValue(true);
        builder.Property(pa => pa.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(pa => pa.ModifiedAt).HasColumnName("modified_at");
        builder.Property(pa => pa.ModifiedBy).HasColumnName("modified_by");

        // Constraints
        builder.HasIndex(pa => new { pa.PatientId, pa.UserId, pa.AccessLevel }).IsUnique().HasDatabaseName("uk_patient_accesses_unique");

        // Check constraints
        builder.HasCheckConstraint("ck_patient_accesses_access_level", "access_level IN ('Read', 'Write', 'Admin', 'Emergency', 'Research', 'Analytics')");

        // Indexes
        builder.HasIndex(pa => pa.TenantId).HasDatabaseName("idx_patient_accesses_tenant_id");
        builder.HasIndex(pa => pa.PatientId).HasDatabaseName("idx_patient_accesses_patient_id");
        builder.HasIndex(pa => pa.UserId).HasDatabaseName("idx_patient_accesses_user_id");
        builder.HasIndex(pa => pa.AccessLevel).HasDatabaseName("idx_patient_accesses_access_level");
        builder.HasIndex(pa => pa.GrantedAt).HasDatabaseName("idx_patient_accesses_granted_at");
        builder.HasIndex(pa => pa.ExpiresAt).HasDatabaseName("idx_patient_accesses_expires_at");
        builder.HasIndex(pa => pa.IsEmergencyAccess).HasDatabaseName("idx_patient_accesses_emergency");
    }
}
