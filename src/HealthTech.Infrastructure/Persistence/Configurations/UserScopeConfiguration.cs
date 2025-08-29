using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HealthTech.Domain.Entities;

namespace HealthTech.Infrastructure.Persistence.Configurations;

public class UserScopeConfiguration : IEntityTypeConfiguration<UserScope>
{
    public void Configure(EntityTypeBuilder<UserScope> builder)
    {
        builder.ToTable("user_scopes");

        builder.HasKey(us => us.Id);
        builder.Property(us => us.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(us => us.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(us => us.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(us => us.Scope).HasColumnName("scope").HasMaxLength(200).IsRequired();
        builder.Property(us => us.GrantedAt).HasColumnName("granted_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(us => us.ExpiresAt).HasColumnName("expires_at");
        builder.Property(us => us.GrantedBy).HasColumnName("granted_by").IsRequired();
        builder.Property(us => us.IsRevoked).HasColumnName("is_revoked").HasDefaultValue(false);
        builder.Property(us => us.RevokedAt).HasColumnName("revoked_at");
        builder.Property(us => us.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(us => us.ModifiedAt).HasColumnName("modified_at");
        builder.Property(us => us.ModifiedBy).HasColumnName("modified_by");
        builder.Property(us => us.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Property(us => us.DeletedAt).HasColumnName("deleted_at");
        builder.Property(us => us.DeletedBy).HasColumnName("deleted_by");
        builder.Property(us => us.Version).HasColumnName("version").HasDefaultValue(1);
        builder.Property(us => us.CreatedBy).HasColumnName("createdby");

        // Constraints
        builder.HasIndex(us => new { us.UserId, us.Scope }).IsUnique().HasDatabaseName("uk_user_scopes_unique");

        // Indexes
        builder.HasIndex(us => us.TenantId).HasDatabaseName("idx_user_scopes_tenant_id");
        builder.HasIndex(us => us.UserId).HasDatabaseName("idx_user_scopes_user_id");
        builder.HasIndex(us => us.Scope).HasDatabaseName("idx_user_scopes_scope");
        builder.HasIndex(us => us.GrantedAt).HasDatabaseName("idx_user_scopes_granted_at");
        builder.HasIndex(us => us.ExpiresAt).HasDatabaseName("idx_user_scopes_expires_at");
        builder.HasIndex(us => us.IsRevoked).HasDatabaseName("idx_user_scopes_is_revoked");
    }
}
