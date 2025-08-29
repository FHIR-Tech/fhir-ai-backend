using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HealthTech.Domain.Entities;
using HealthTech.Domain.Enums;

namespace HealthTech.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(u => u.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(u => u.Username).HasColumnName("username").HasMaxLength(100).IsRequired();
        builder.Property(u => u.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
        builder.Property(u => u.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
        builder.Property(u => u.Role).HasColumnName("role").HasConversion<string>().IsRequired();
        builder.Property(u => u.Status).HasColumnName("status").HasConversion<string>().HasDefaultValue(UserStatus.Active).IsRequired();
        builder.Property(u => u.FirstName).HasColumnName("first_name").HasMaxLength(100);
        builder.Property(u => u.LastName).HasColumnName("last_name").HasMaxLength(100);
        builder.Property(u => u.PractitionerId).HasColumnName("practitioner_id").HasMaxLength(100);

        builder.Property(u => u.LastLoginAt).HasColumnName("last_login_at");
        builder.Property(u => u.LastLoginIp).HasColumnName("last_login_ip");
        builder.Property(u => u.FailedLoginAttempts).HasColumnName("failed_login_attempts").HasDefaultValue(0);
        builder.Property(u => u.LockedUntil).HasColumnName("locked_until");
        builder.Property(u => u.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(u => u.ModifiedAt).HasColumnName("modified_at");
        builder.Property(u => u.CreatedBy).HasColumnName("created_by");
        builder.Property(u => u.ModifiedBy).HasColumnName("modified_by");
        builder.Property(u => u.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Property(u => u.DeletedAt).HasColumnName("deleted_at");
        builder.Property(u => u.DeletedBy).HasColumnName("deleted_by");
        builder.Property(u => u.Version).HasColumnName("version").HasDefaultValue(1);

        // Constraints
        builder.HasIndex(u => new { u.TenantId, u.Username }).IsUnique().HasDatabaseName("uk_users_tenant_username");
        builder.HasIndex(u => new { u.TenantId, u.Email }).IsUnique().HasDatabaseName("uk_users_tenant_email");
        builder.HasIndex(u => u.PractitionerId).IsUnique().HasDatabaseName("uk_users_practitioner_id");

        // Check constraints
        builder.HasCheckConstraint("ck_users_role", "role IN ('SystemAdministrator', 'HealthcareProvider', 'Nurse', 'Patient', 'FamilyMember', 'Researcher', 'ITSupport', 'ReadOnlyUser', 'DataAnalyst', 'ITAdministrator', 'Guest')");
        builder.HasCheckConstraint("ck_users_status", "status IN ('Active', 'Inactive', 'Locked', 'Suspended', 'Pending', 'Expired', 'PendingVerification', 'Deleted')");

        // Indexes
        builder.HasIndex(u => u.TenantId).HasDatabaseName("idx_users_tenant_id");
        builder.HasIndex(u => u.Username).HasDatabaseName("idx_users_username");
        builder.HasIndex(u => u.Email).HasDatabaseName("idx_users_email");
        builder.HasIndex(u => u.Role).HasDatabaseName("idx_users_role");
        builder.HasIndex(u => u.Status).HasDatabaseName("idx_users_status");
        builder.HasIndex(u => u.PractitionerId).HasDatabaseName("idx_users_practitioner_id");
    }
}
