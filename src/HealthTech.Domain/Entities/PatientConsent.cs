using System.ComponentModel.DataAnnotations;

namespace HealthTech.Domain.Entities;

/// <summary>
/// Patient consent entity for managing patient consent and authorization
/// </summary>
public class PatientConsent : BaseEntity
{
    /// <summary>
    /// Patient ID
    /// </summary>
    [Required]
    public Guid PatientId { get; set; }

    /// <summary>
    /// Type of consent
    /// </summary>
    [Required]
    public ConsentType ConsentType { get; set; }

    /// <summary>
    /// Whether consent is currently active
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// When consent was granted
    /// </summary>
    [Required]
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When consent expires (null = no expiration)
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// User who granted the consent
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string GrantedBy { get; set; } = string.Empty;

    /// <summary>
    /// User who revoked the consent (if applicable)
    /// </summary>
    [MaxLength(255)]
    public string? RevokedBy { get; set; }

    /// <summary>
    /// When consent was revoked (if applicable)
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Purpose of the consent
    /// </summary>
    public string? Purpose { get; set; }

    /// <summary>
    /// Additional details about the consent
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Whether consent was given electronically
    /// </summary>
    public bool IsElectronicConsent { get; set; }

    /// <summary>
    /// IP address where consent was given
    /// </summary>
    [MaxLength(45)]
    public string? ConsentIpAddress { get; set; }

    /// <summary>
    /// User agent/browser information
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Whether consent is currently valid
    /// </summary>
    public bool IsValid => IsActive && (ExpiresAt == null || ExpiresAt > DateTime.UtcNow) && RevokedAt == null;

    /// <summary>
    /// Navigation property for patient
    /// </summary>
    public virtual Patient Patient { get; set; } = null!;
}

/// <summary>
/// Types of patient consent
/// </summary>
public enum ConsentType
{
    /// <summary>
    /// Consent for data sharing with healthcare providers
    /// </summary>
    DataSharing,

    /// <summary>
    /// Consent for research participation
    /// </summary>
    ResearchParticipation,

    /// <summary>
    /// Consent for emergency access
    /// </summary>
    EmergencyAccess,

    /// <summary>
    /// Consent for family member access
    /// </summary>
    FamilyAccess,

    /// <summary>
    /// Consent for third-party access
    /// </summary>
    ThirdPartyAccess,

    /// <summary>
    /// Consent for marketing communications
    /// </summary>
    MarketingCommunications,

    /// <summary>
    /// Consent for automated decision making
    /// </summary>
    AutomatedDecisionMaking,

    /// <summary>
    /// Consent for data portability
    /// </summary>
    DataPortability,

    /// <summary>
    /// Consent for data retention
    /// </summary>
    DataRetention,

    /// <summary>
    /// General treatment consent
    /// </summary>
    TreatmentConsent
}
