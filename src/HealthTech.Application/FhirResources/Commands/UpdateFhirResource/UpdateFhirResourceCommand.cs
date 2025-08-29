using MediatR;

namespace HealthTech.Application.FhirResources.Commands.UpdateFhirResource;

/// <summary>
/// Command to update an existing FHIR resource
/// </summary>
public record UpdateFhirResourceCommand : IRequest<UpdateFhirResourceResponse>
{
    /// <summary>
    /// FHIR resource type
    /// </summary>
    public string ResourceType { get; init; } = string.Empty;

    /// <summary>
    /// FHIR resource ID
    /// </summary>
    public string FhirId { get; init; } = string.Empty;

    /// <summary>
    /// FHIR resource JSON content
    /// </summary>
    public string ResourceJson { get; init; } = string.Empty;
}

/// <summary>
/// Response for updating FHIR resource
/// </summary>
public record UpdateFhirResourceResponse
{
    /// <summary>
    /// Updated FHIR resource ID
    /// </summary>
    public string FhirId { get; init; } = string.Empty;

    /// <summary>
    /// Updated FHIR resource version
    /// </summary>
    public int VersionId { get; init; }

    /// <summary>
    /// Updated FHIR resource JSON
    /// </summary>
    public string ResourceJson { get; init; } = string.Empty;
}
