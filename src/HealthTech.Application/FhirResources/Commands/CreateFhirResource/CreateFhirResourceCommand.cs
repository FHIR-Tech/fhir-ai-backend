using MediatR;
using HealthTech.Application.Common.Interfaces;

namespace HealthTech.Application.FhirResources.Commands.CreateFhirResource;

/// <summary>
/// Command to create a new FHIR resource
/// </summary>
public record CreateFhirResourceCommand : IRequest<CreateFhirResourceResponse>
{
    /// <summary>
    /// FHIR resource type
    /// </summary>
    public string ResourceType { get; init; } = string.Empty;

    /// <summary>
    /// FHIR resource JSON content
    /// </summary>
    public string ResourceJson { get; init; } = string.Empty;

    /// <summary>
    /// FHIR resource ID (optional, will be generated if not provided)
    /// </summary>
    public string? FhirId { get; init; }
}

/// <summary>
/// Response for creating FHIR resource
/// </summary>
public record CreateFhirResourceResponse
{
    /// <summary>
    /// Created FHIR resource ID
    /// </summary>
    public string FhirId { get; init; } = string.Empty;

    /// <summary>
    /// FHIR resource version
    /// </summary>
    public int VersionId { get; init; }

    /// <summary>
    /// Created FHIR resource JSON
    /// </summary>
    public string ResourceJson { get; init; } = string.Empty;
}
