using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthTech.Domain.Entities;

/// <summary>
/// Patient entity for managing patient information and access control
/// </summary>
public class Patient : BaseEntity
{
    // ========================================
    // FHIR INTEGRATION FIELDS
    // ========================================
    
    /// <summary>
    /// FHIR Patient ID
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string FhirPatientId { get; set; } = string.Empty;

    // ========================================
    // CORE IDENTITY FIELDS
    // ========================================
    
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

    /// <summary>
    /// Date of birth
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Gender (male, female, other, unknown)
    /// </summary>
    [MaxLength(10)]
    public string Gender { get; set; } = string.Empty;

    // ========================================
    // CONTACT INFORMATION FIELDS
    // ========================================
    
    /// <summary>
    /// Email address
    /// </summary>
    [MaxLength(255)]
    [EmailAddress]
    public string? Email { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    [MaxLength(50)]
    public string? Phone { get; set; }

    /// <summary>
    /// Address
    /// </summary>
    public string? Address { get; set; }

    // ========================================
    // STATUS & CONFIGURATION FIELDS
    // ========================================
    
    /// <summary>
    /// Patient status
    /// </summary>
    [Required]
    public PatientStatus Status { get; set; } = PatientStatus.Active;

    // ========================================
    // CONSENT FIELDS
    // ========================================
    
    /// <summary>
    /// Whether patient has given consent for data sharing
    /// </summary>
    public bool ConsentGiven { get; set; }

    /// <summary>
    /// Date when consent was given
    /// </summary>
    public DateTime? ConsentDate { get; set; }

    /// <summary>
    /// User who recorded the consent
    /// </summary>
    [MaxLength(255)]
    public string? ConsentGivenBy { get; set; }

    // ========================================
    // EMERGENCY CONTACT FIELDS
    // ========================================
    
    /// <summary>
    /// Emergency contact name
    /// </summary>
    [MaxLength(255)]
    public string? EmergencyContactName { get; set; }

    /// <summary>
    /// Emergency contact phone
    /// </summary>
    [MaxLength(50)]
    public string? EmergencyContactPhone { get; set; }

    /// <summary>
    /// Relationship to emergency contact
    /// </summary>
    [MaxLength(100)]
    public string? EmergencyContactRelationship { get; set; }

    // ========================================
    // COMPUTED PROPERTIES
    // ========================================
    
    /// <summary>
    /// Patient's display name (computed property)
    /// </summary>
    [NotMapped]
    public string DisplayName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Patient's age (computed property)
    /// </summary>
    [NotMapped]
    public int? Age => DateOfBirth?.Year > 0 ? DateTime.Now.Year - DateOfBirth.Value.Year : null;

    // ========================================
    // NAVIGATION PROPERTIES
    // ========================================
    
    /// <summary>
    /// Navigation property for patient access
    /// </summary>
    public virtual ICollection<PatientAccess> PatientAccesses { get; set; } = new List<PatientAccess>();

    /// <summary>
    /// Navigation property for patient consents
    /// </summary>
    public virtual ICollection<PatientConsent> PatientConsents { get; set; } = new List<PatientConsent>();
}

/// <summary>
/// Patient status
/// </summary>
public enum PatientStatus
{
    /// <summary>
    /// Active patient
    /// </summary>
    Active,

    /// <summary>
    /// Inactive patient
    /// </summary>
    Inactive,

    /// <summary>
    /// Deceased patient
    /// </summary>
    Deceased,

    /// <summary>
    /// Unknown status
    /// </summary>
    Unknown,

    /// <summary>
    /// Transferred to another facility
    /// </summary>
    Transferred,

    /// <summary>
    /// Discharged
    /// </summary>
    Discharged
}
