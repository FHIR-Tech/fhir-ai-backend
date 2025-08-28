using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HealthTech.Domain.Entities;
using HealthTech.Domain.Enums;

namespace HealthTech.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("patients");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(p => p.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(p => p.FhirPatientId).HasColumnName("fhir_patient_id").HasMaxLength(100).IsRequired();
        builder.Property(p => p.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
        builder.Property(p => p.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
        builder.Property(p => p.DateOfBirth).HasColumnName("date_of_birth").IsRequired();
        builder.Property(p => p.Gender).HasColumnName("gender").HasMaxLength(10);
        builder.Property(p => p.Status).HasColumnName("status").HasConversion<string>().HasDefaultValue(PatientStatus.Active).IsRequired();
        builder.Property(p => p.ConsentGiven).HasColumnName("consent_given").HasDefaultValue(false);
        builder.Property(p => p.EmergencyContactName).HasColumnName("emergency_contact_name").HasMaxLength(200);
        builder.Property(p => p.EmergencyContactPhone).HasColumnName("emergency_contact_phone").HasMaxLength(50);
        builder.Property(p => p.EmergencyContactRelationship).HasColumnName("emergency_contact_relationship").HasMaxLength(100);


        builder.Property(p => p.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(p => p.ModifiedAt).HasColumnName("modified_at");
        builder.Property(p => p.CreatedBy).HasColumnName("created_by");
        builder.Property(p => p.ModifiedBy).HasColumnName("modified_by");

        // Constraints
        builder.HasIndex(p => new { p.TenantId, p.FhirPatientId }).IsUnique().HasDatabaseName("uk_patients_tenant_fhir_id");

        // Check constraints
        builder.HasCheckConstraint("ck_patients_gender", "gender IN ('Male', 'Female', 'Other', 'Unknown')");
        builder.HasCheckConstraint("ck_patients_status", "status IN ('Active', 'Inactive', 'Deceased', 'Unknown', 'Transferred', 'Discharged')");

        // Foreign key relationships


        // Indexes
        builder.HasIndex(p => p.TenantId).HasDatabaseName("idx_patients_tenant_id");
        builder.HasIndex(p => p.FhirPatientId).HasDatabaseName("idx_patients_fhir_patient_id");
        builder.HasIndex(p => new { p.LastName, p.FirstName }).HasDatabaseName("idx_patients_name");
        builder.HasIndex(p => p.Status).HasDatabaseName("idx_patients_status");
        builder.HasIndex(p => p.ConsentGiven).HasDatabaseName("idx_patients_consent");
    }
}
