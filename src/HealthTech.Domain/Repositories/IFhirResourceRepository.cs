using HealthTech.Domain.Entities;

namespace HealthTech.Domain.Repositories;

/// <summary>
/// Repository interface for FHIR resources
/// </summary>
public interface IFhirResourceRepository : IRepository<FhirResource>
{
    /// <summary>
    /// Get FHIR resource by type and ID
    /// </summary>
    /// <param name="resourceType">FHIR resource type</param>
    /// <param name="fhirId">FHIR resource ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>FHIR resource or null if not found</returns>
    Task<FhirResource?> GetByFhirIdAsync(string resourceType, string fhirId, string tenantId);

    /// <summary>
    /// Get FHIR resources by type
    /// </summary>
    /// <param name="resourceType">FHIR resource type</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <param name="skip">Number of records to skip</param>
    /// <param name="take">Number of records to take</param>
    /// <returns>Collection of FHIR resources</returns>
    Task<IEnumerable<FhirResource>> GetByTypeAsync(string resourceType, string tenantId, int skip = 0, int take = 100);

    /// <summary>
    /// Search FHIR resources by parameters
    /// </summary>
    /// <param name="resourceType">FHIR resource type</param>
    /// <param name="searchParameters">Search parameters</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <param name="skip">Number of records to skip</param>
    /// <param name="take">Number of records to take</param>
    /// <returns>Collection of FHIR resources</returns>
    Task<IEnumerable<FhirResource>> SearchAsync(string resourceType, Dictionary<string, string> searchParameters, string tenantId, int skip = 0, int take = 100);

    /// <summary>
    /// Get FHIR resource history
    /// </summary>
    /// <param name="resourceType">FHIR resource type</param>
    /// <param name="fhirId">FHIR resource ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>Collection of FHIR resource versions</returns>
    Task<IEnumerable<FhirResource>> GetHistoryAsync(string resourceType, string fhirId, string tenantId);

    /// <summary>
    /// Get FHIR resource by version
    /// </summary>
    /// <param name="resourceType">FHIR resource type</param>
    /// <param name="fhirId">FHIR resource ID</param>
    /// <param name="versionId">Version ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>FHIR resource or null if not found</returns>
    Task<FhirResource?> GetByVersionAsync(string resourceType, string fhirId, int versionId, string tenantId);

    /// <summary>
    /// Get FHIR resource by type and ID (alias for GetByFhirIdAsync)
    /// </summary>
    /// <param name="resourceType">FHIR resource type</param>
    /// <param name="fhirId">FHIR resource ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>FHIR resource or null if not found</returns>
    Task<FhirResource?> GetByTypeAndIdAsync(string resourceType, string fhirId, CancellationToken cancellationToken);
}
