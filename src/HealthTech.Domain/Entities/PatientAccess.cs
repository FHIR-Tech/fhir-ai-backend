using System.ComponentModel.DataAnnotations;
using HealthTech.Domain.Enums;

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
    public PatientAccessLevel AccessLevel { get; set; }

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
    /// Whether access is manually enabled/disabled
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Whether access is currently active (computed based on IsEnabled and ExpiresAt)
    /// </summary>
    public bool IsActive => IsEnabled && (ExpiresAt == null || ExpiresAt > DateTime.UtcNow);

    /// <summary>
    /// Navigation property for patient
    /// </summary>
    public virtual Patient Patient { get; set; } = null!;

    /// <summary>
    /// Navigation property for user
    /// </summary>
    public virtual User User { get; set; } = null!;
}


