using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HealthTech.Domain.Entities;
using HealthTech.Domain.Enums;

namespace HealthTech.Infrastructure.Persistence.Configurations;

public class PatientConsentConfiguration : IEntityTypeConfiguration<PatientConsent>
{
    public void Configure(EntityTypeBuilder<PatientConsent> builder)
    {
        builder.ToTable("patient_consents");

        builder.HasKey(pc => pc.Id);
        builder.Property(pc => pc.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(pc => pc.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(pc => pc.PatientId).HasColumnName("patient_id").IsRequired();
        builder.Property(pc => pc.ConsentType).HasColumnName("consent_type").HasConversion<string>().IsRequired();
        builder.Property(pc => pc.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(pc => pc.GrantedAt).HasColumnName("granted_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(pc => pc.ExpiresAt).HasColumnName("expires_at");
        builder.Property(pc => pc.GrantedBy).HasColumnName("granted_by").IsRequired();
        builder.Property(pc => pc.RevokedBy).HasColumnName("revoked_by");
        builder.Property(pc => pc.RevokedAt).HasColumnName("revoked_at");
        builder.Property(pc => pc.Purpose).HasColumnName("purpose").IsRequired();
        builder.Property(pc => pc.Details).HasColumnName("details");
        builder.Property(pc => pc.IsElectronicConsent).HasColumnName("is_electronic_consent").HasDefaultValue(false);
        builder.Property(pc => pc.ConsentIpAddress).HasColumnName("consent_ip_address");
        builder.Property(pc => pc.UserAgent).HasColumnName("user_agent");
        builder.Property(pc => pc.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(pc => pc.ModifiedAt).HasColumnName("modified_at");
        builder.Property(pc => pc.ModifiedBy).HasColumnName("modified_by");

        // Constraints
        builder.HasIndex(pc => new { pc.PatientId, pc.ConsentType, pc.IsActive }).HasDatabaseName("idx_patient_consents_active");

        // Check constraints
        builder.HasCheckConstraint("ck_patient_consents_consent_type", "consent_type IN ('DataSharing', 'ResearchParticipation', 'EmergencyAccess', 'FamilyAccess', 'ThirdPartyAccess', 'MarketingCommunications', 'AutomatedDecisionMaking', 'DataPortability', 'DataRetention', 'TreatmentConsent')");

        // Indexes
        builder.HasIndex(pc => pc.TenantId).HasDatabaseName("idx_patient_consents_tenant_id");
        builder.HasIndex(pc => pc.PatientId).HasDatabaseName("idx_patient_consents_patient_id");
        builder.HasIndex(pc => pc.ConsentType).HasDatabaseName("idx_patient_consents_consent_type");
        builder.HasIndex(pc => pc.GrantedAt).HasDatabaseName("idx_patient_consents_granted_at");
        builder.HasIndex(pc => pc.ExpiresAt).HasDatabaseName("idx_patient_consents_expires_at");
        builder.HasIndex(pc => pc.IsActive).HasDatabaseName("idx_patient_consents_is_active");
    }
}
