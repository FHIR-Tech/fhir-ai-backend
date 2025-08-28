using System.ComponentModel.DataAnnotations;

namespace HealthTech.Domain.Entities;

/// <summary>
/// User session entity for managing user sessions and tokens
/// </summary>
public class UserSession : BaseEntity
{
    /// <summary>
    /// User ID
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Session token
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string SessionToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token
    /// </summary>
    [MaxLength(500)]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// When session was created
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When session expires
    /// </summary>
    [Required]
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// When session was last accessed
    /// </summary>
    public DateTime? LastAccessedAt { get; set; }

    /// <summary>
    /// IP address where session was created
    /// </summary>
    [MaxLength(45)]
    public string? CreatedIpAddress { get; set; }

    /// <summary>
    /// User agent/browser information
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Whether session is currently active (computed based on IsRevoked and ExpiresAt)
    /// </summary>
    public bool IsActive => !IsRevoked && ExpiresAt > DateTime.UtcNow;

    /// <summary>
    /// Whether session was revoked
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// When session was revoked
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Reason for revocation
    /// </summary>
    public string? RevocationReason { get; set; }

    /// <summary>
    /// Navigation property for user
    /// </summary>
    public virtual User User { get; set; } = null!;
}
