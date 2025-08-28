using System.ComponentModel.DataAnnotations;

namespace HealthTech.Domain.Entities;

/// <summary>
/// Patient access control entity for managing access to patient data
/// </summary>
public class PatientAccess : BaseEntity
{
    /// <summary>
    /// Patient ID
    /// </summary>
    [Required]
    public Guid PatientId { get; set; }

    /// <summary>
    /// User ID who has access
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Level of access granted
    /// </summary>
    [Required]
    public AccessLevel AccessLevel { get; set; }

    /// <summary>
    /// When access was granted
    /// </summary>
    [Required]
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When access expires (null = no expiration)
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// User who granted the access
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string GrantedBy { get; set; } = string.Empty;

    /// <summary>
    /// Reason for granting access
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Whether this is an emergency access
    /// </summary>
    public bool IsEmergencyAccess { get; set; }

    /// <summary>
    /// Emergency access justification (required if IsEmergencyAccess = true)
    /// </summary>
    public string? EmergencyJustification { get; set; }

    /// <summary>
    /// Whether access is currently active
    /// </summary>
    public bool IsActive => ExpiresAt == null || ExpiresAt > DateTime.UtcNow;

    /// <summary>
    /// Navigation property for patient
    /// </summary>
    public virtual Patient Patient { get; set; } = null!;

    /// <summary>
    /// Navigation property for user
    /// </summary>
    public virtual User User { get; set; } = null!;
}

/// <summary>
/// Access levels for patient data
/// </summary>
public enum AccessLevel
{
    /// <summary>
    /// Read-only access to patient data
    /// </summary>
    ReadOnly,

    /// <summary>
    /// Read and write access to patient data
    /// </summary>
    ReadWrite,

    /// <summary>
    /// Full access including administrative functions
    /// </summary>
    FullAccess,

    /// <summary>
    /// Emergency access with limited scope
    /// </summary>
    EmergencyAccess,

    /// <summary>
    /// Research access with anonymized data
    /// </summary>
    ResearchAccess,

    /// <summary>
    /// Family member access with restrictions
    /// </summary>
    FamilyAccess
}
