using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthTech.Domain.Entities;

/// <summary>
/// User scope entity for managing user permissions and scopes
/// </summary>
public class UserScope : BaseEntity
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
    // CORE SCOPE FIELDS
    // ========================================
    
    /// <summary>
    /// Scope name (e.g., "patient/*", "user/*", "system/*")
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Scope { get; set; } = string.Empty;

    // ========================================
    // TIMING FIELDS
    // ========================================
    
    /// <summary>
    /// When scope was granted
    /// </summary>
    [Required]
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When scope expires (null = no expiration)
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    // ========================================
    // AUTHORIZATION FIELDS
    // ========================================
    
    /// <summary>
    /// User who granted the scope
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string GrantedBy { get; set; } = string.Empty;

    // ========================================
    // REVOCATION FIELDS
    // ========================================
    
    /// <summary>
    /// Whether scope was manually revoked
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// When scope was revoked
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    // ========================================
    // COMPUTED PROPERTIES
    // ========================================
    
    /// <summary>
    /// Whether scope is currently active (computed based on IsRevoked and ExpiresAt)
    /// </summary>
    [NotMapped]
    public bool IsActive => !IsRevoked && (ExpiresAt == null || ExpiresAt > DateTime.UtcNow);

    // ========================================
    // NAVIGATION PROPERTIES
    // ========================================
    
    /// <summary>
    /// Navigation property for user
    /// </summary>
    public virtual User User { get; set; } = null!;
}
