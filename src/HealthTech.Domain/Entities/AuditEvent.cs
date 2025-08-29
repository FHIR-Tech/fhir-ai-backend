using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthTech.Domain.Entities;

/// <summary>
/// Audit event entity for tracking all system activities
/// </summary>
public class AuditEvent : BaseEntity
{
    // ========================================
    // CORE EVENT FIELDS
    // ========================================
    
    /// <summary>
    /// Type of audit event
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Subtype of the event
    /// </summary>
    [MaxLength(100)]
    public string? EventSubtype { get; set; }

    /// <summary>
    /// Action performed (C, R, U, D, E)
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Outcome of the action (0=success, 4=minor, 8=major, 12=serious)
    /// </summary>
    public int Outcome { get; set; }

    /// <summary>
    /// Description of the event
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    // ========================================
    // USER CONTEXT FIELDS
    // ========================================
    
    /// <summary>
    /// User who performed the action
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// User's display name
    /// </summary>
    [MaxLength(255)]
    public string? UserDisplayName { get; set; }

    /// <summary>
    /// IP address of the user
    /// </summary>
    [MaxLength(45)]
    public string? UserIpAddress { get; set; }

    // ========================================
    // RESOURCE CONTEXT FIELDS
    // ========================================
    
    /// <summary>
    /// Resource type that was affected
    /// </summary>
    [MaxLength(100)]
    public string? ResourceType { get; set; }

    /// <summary>
    /// Resource ID that was affected
    /// </summary>
    [MaxLength(255)]
    public string? ResourceId { get; set; }

    // ========================================
    // ADDITIONAL DATA FIELDS
    // ========================================
    
    /// <summary>
    /// Additional data about the event
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string? EventData { get; set; }

    // ========================================
    // TIMING FIELDS
    // ========================================
    
    /// <summary>
    /// Timestamp when the event occurred
    /// </summary>
    public DateTime EventTime { get; set; }
}
