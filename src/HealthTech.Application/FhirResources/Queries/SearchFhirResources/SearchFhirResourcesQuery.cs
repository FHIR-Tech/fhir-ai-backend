using MediatR;

namespace HealthTech.Application.FhirResources.Queries.SearchFhirResources;

/// <summary>
/// Query to search FHIR resources
/// </summary>
public record SearchFhirResourcesQuery : IRequest<SearchFhirResourcesResponse>
{
    /// <summary>
    /// FHIR resource type
    /// </summary>
    public string ResourceType { get; init; } = string.Empty;

    /// <summary>
    /// Search parameters
    /// </summary>
    public Dictionary<string, string> SearchParameters { get; init; } = new();

    /// <summary>
    /// Number of records to skip
    /// </summary>
    public int Skip { get; init; } = 0;

    /// <summary>
    /// Number of records to take
    /// </summary>
    public int Take { get; init; } = 100;
}

/// <summary>
/// Response for searching FHIR resources
/// </summary>
public record SearchFhirResourcesResponse
{
    /// <summary>
    /// Collection of FHIR resources
    /// </summary>
    public IEnumerable<FhirResourceDto> Resources { get; init; } = Enumerable.Empty<FhirResourceDto>();

    /// <summary>
    /// Total count of resources
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// FHIR resource DTO
    /// </summary>
    public record FhirResourceDto
    {
        /// <summary>
        /// FHIR resource ID
        /// </summary>
        public string FhirId { get; init; } = string.Empty;

        /// <summary>
        /// FHIR resource version
        /// </summary>
        public int VersionId { get; init; }

        /// <summary>
        /// FHIR resource JSON
        /// </summary>
        public string ResourceJson { get; init; } = string.Empty;

        /// <summary>
        /// Last updated timestamp
        /// </summary>
        public DateTime? LastUpdated { get; init; }
    }
}
