using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthTech.Domain.Entities;

/// <summary>
/// User session entity for managing user sessions and tokens
/// </summary>
public class UserSession : BaseEntity
{
    // ========================================
    // FOREIGN KEY FIELDS
    // ========================================
    
    /// <summary>
    /// User ID
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    // ========================================
    // CORE SESSION FIELDS
    // ========================================
    
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

    // ========================================
    // TIMING FIELDS
    // ========================================
    
    /// <summary>
    /// When session expires
    /// </summary>
    [Required]
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// When session was last accessed
    /// </summary>
    public DateTime? LastAccessedAt { get; set; }

    // ========================================
    // SECURITY & TRACKING FIELDS
    // ========================================
    
    /// <summary>
    /// IP address where session was created
    /// </summary>
    [MaxLength(45)]
    public string? CreatedIpAddress { get; set; }

    /// <summary>
    /// User agent/browser information
    /// </summary>
    public string? UserAgent { get; set; }

    // ========================================
    // REVOCATION FIELDS
    // ========================================
    
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

    // ========================================
    // COMPUTED PROPERTIES
    // ========================================
    
    /// <summary>
    /// Whether session is currently active (computed based on IsRevoked and ExpiresAt)
    /// </summary>
    [NotMapped]
    public bool IsActive => !IsRevoked && ExpiresAt > DateTime.UtcNow;

    // ========================================
    // NAVIGATION PROPERTIES
    // ========================================
    
    /// <summary>
    /// Navigation property for user
    /// </summary>
    public virtual User User { get; set; } = null!;
}
