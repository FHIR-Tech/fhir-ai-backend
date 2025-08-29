using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthTech.Domain.Enums;

namespace HealthTech.Domain.Entities;

/// <summary>
/// User entity for managing user accounts and authentication
/// </summary>
public class User : BaseEntity
{
    // ========================================
    // CORE IDENTITY FIELDS
    // ========================================
    
    /// <summary>
    /// Username for login
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Email address
    /// </summary>
    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// First name
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string LastName { get; set; } = string.Empty;

    // ========================================
    // STATUS & CONFIGURATION FIELDS
    // ========================================
    
    /// <summary>
    /// User role in the system
    /// </summary>
    [Required]
    public UserRole Role { get; set; }

    /// <summary>
    /// Current status of the user account
    /// </summary>
    [Required]
    public UserStatus Status { get; set; } = UserStatus.Active;

    // ========================================
    // SECURITY & AUTHENTICATION FIELDS
    // ========================================
    
    /// <summary>
    /// Hashed password
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Number of failed login attempts
    /// </summary>
    public int FailedLoginAttempts { get; set; }

    /// <summary>
    /// Account locked until this timestamp
    /// </summary>
    public DateTime? LockedUntil { get; set; }

    // ========================================
    // FHIR INTEGRATION FIELDS
    // ========================================
    
    /// <summary>
    /// FHIR Practitioner ID reference
    /// </summary>
    [MaxLength(255)]
    public string? PractitionerId { get; set; }

    // ========================================
    // TIMING & TRACKING FIELDS
    // ========================================
    
    /// <summary>
    /// Last login timestamp
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// IP address of last login
    /// </summary>
    [MaxLength(45)]
    public string? LastLoginIp { get; set; }

    // ========================================
    // COMPUTED PROPERTIES
    // ========================================
    
    /// <summary>
    /// User's display name (computed property)
    /// </summary>
    [NotMapped]
    public string DisplayName => $"{FirstName} {LastName}".Trim();

    // ========================================
    // NAVIGATION PROPERTIES
    // ========================================
    
    /// <summary>
    /// Navigation property for user scopes
    /// </summary>
    public virtual ICollection<UserScope> UserScopes { get; set; } = new List<UserScope>();

    /// <summary>
    /// Navigation property for user sessions
    /// </summary>
    public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();

    /// <summary>
    /// Navigation property for patient access
    /// </summary>
    public virtual ICollection<PatientAccess> PatientAccesses { get; set; } = new List<PatientAccess>();
}
