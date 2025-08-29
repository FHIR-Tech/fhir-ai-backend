using MediatR;

namespace HealthTech.Application.FhirResources.Commands;

/// <summary>
/// Command to delete (soft delete) a FHIR resource
/// </summary>
public record DeleteFhirResourceCommand : IRequest<DeleteFhirResourceResponse>
{
    /// <summary>
    /// FHIR resource type
    /// </summary>
    public string ResourceType { get; init; } = string.Empty;

    /// <summary>
    /// FHIR resource ID
    /// </summary>
    public string FhirId { get; init; } = string.Empty;
}

/// <summary>
/// Response for deleting FHIR resource
/// </summary>
public record DeleteFhirResourceResponse
{
    /// <summary>
    /// Deleted FHIR resource ID
    /// </summary>
    public string FhirId { get; init; } = string.Empty;

    /// <summary>
    /// Deletion timestamp
    /// </summary>
    public DateTime DeletedAt { get; init; }

    /// <summary>
    /// Success indicator
    /// </summary>
    public bool Success { get; init; }
}
