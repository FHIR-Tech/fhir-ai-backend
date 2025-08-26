using System.ComponentModel.DataAnnotations;

namespace HealthTech.Domain.Entities;

/// <summary>
/// Base entity class for all domain entities
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Tenant identifier for multi-tenancy support
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Date when the entity was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// User who created the entity
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Date when the entity was last modified
    /// </summary>
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// User who last modified the entity
    /// </summary>
    public string? ModifiedBy { get; set; }

    /// <summary>
    /// Soft delete flag
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Date when the entity was deleted (soft delete)
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// User who deleted the entity
    /// </summary>
    [MaxLength(255)]
    public string? DeletedBy { get; set; }

    /// <summary>
    /// Version for optimistic concurrency control
    /// </summary>
    public int Version { get; set; }
}
