using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthTech.Domain.Entities;

/// <summary>
/// Entity for storing FHIR resources in PostgreSQL JSONB format
/// </summary>
public class FhirResource : BaseEntity
{
    /// <summary>
    /// FHIR resource type (e.g., Patient, Observation, etc.)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string ResourceType { get; set; } = string.Empty;

    /// <summary>
    /// FHIR resource ID
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string FhirId { get; set; } = string.Empty;

    /// <summary>
    /// FHIR resource version
    /// </summary>
    public int VersionId { get; set; }

    /// <summary>
    /// FHIR resource as JSONB for efficient querying
    /// </summary>
    [Required]
    [Column(TypeName = "jsonb")]
    public string ResourceJson { get; set; } = string.Empty;

    /// <summary>
    /// Status of the FHIR resource
    /// </summary>
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Last updated timestamp from FHIR
    /// </summary>
    public DateTime? LastUpdated { get; set; }

    /// <summary>
    /// Search parameters for efficient querying
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string? SearchParameters { get; set; }

    /// <summary>
    /// Tags for categorization
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string? Tags { get; set; }

    /// <summary>
    /// Security labels for access control
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string? SecurityLabels { get; set; }
}
