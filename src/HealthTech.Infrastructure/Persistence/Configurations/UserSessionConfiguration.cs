using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HealthTech.Domain.Entities;

namespace HealthTech.Infrastructure.Persistence.Configurations;

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("user_sessions");

        builder.HasKey(us => us.Id);
        builder.Property(us => us.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(us => us.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(us => us.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(us => us.SessionToken).HasColumnName("session_token").HasMaxLength(500).IsRequired();
        builder.Property(us => us.RefreshToken).HasColumnName("refresh_token").HasMaxLength(500);
        builder.Property(us => us.ExpiresAt).HasColumnName("expires_at").IsRequired();
        builder.Property(us => us.LastAccessedAt).HasColumnName("last_accessed_at");
        builder.Property(us => us.CreatedIpAddress).HasColumnName("created_ip_address");
        builder.Property(us => us.UserAgent).HasColumnName("user_agent");
        builder.Property(us => us.IsRevoked).HasColumnName("is_revoked").HasDefaultValue(false);
        builder.Property(us => us.RevokedAt).HasColumnName("revoked_at");
        builder.Property(us => us.RevocationReason).HasColumnName("revocation_reason");
        builder.Property(us => us.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(us => us.ModifiedAt).HasColumnName("modified_at");
        builder.Property(us => us.ModifiedBy).HasColumnName("modified_by");

        // Constraints
        builder.HasIndex(us => us.SessionToken).IsUnique().HasDatabaseName("uk_user_sessions_session_token");
        builder.HasIndex(us => us.RefreshToken).HasDatabaseName("idx_user_sessions_refresh_token");

        // Indexes
        builder.HasIndex(us => us.TenantId).HasDatabaseName("idx_user_sessions_tenant_id");
        builder.HasIndex(us => us.UserId).HasDatabaseName("idx_user_sessions_user_id");
        builder.HasIndex(us => us.ExpiresAt).HasDatabaseName("idx_user_sessions_expires_at");
        builder.HasIndex(us => us.LastAccessedAt).HasDatabaseName("idx_user_sessions_last_accessed_at");
        builder.HasIndex(us => us.IsRevoked).HasDatabaseName("idx_user_sessions_is_revoked");
    }
}
