using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Entities;
using HealthTech.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HealthTech.Infrastructure.Repositories;

/// <summary>
/// FHIR resource repository implementation
/// </summary>
public class FhirResourceRepository : IFhirResourceRepository
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Application database context</param>
    public FhirResourceRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get entity by ID
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>Entity or null if not found</returns>
    public async Task<FhirResource?> GetByIdAsync(Guid id, string tenantId)
    {
        return await _context.FhirResources
            .FirstOrDefaultAsync(r => r.Id == id && r.TenantId == tenantId && !r.IsDeleted);
    }

    /// <summary>
    /// Get all entities for a tenant
    /// </summary>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>Collection of entities</returns>
    public async Task<IEnumerable<FhirResource>> GetAllAsync(string tenantId)
    {
        return await _context.FhirResources
            .Where(r => r.TenantId == tenantId && !r.IsDeleted)
            .ToListAsync();
    }

    /// <summary>
    /// Add new entity
    /// </summary>
    /// <param name="entity">Entity to add</param>
    public async Task AddAsync(FhirResource entity)
    {
        _context.FhirResources.Add(entity);
        await _context.SaveChangesAsync(CancellationToken.None);
    }

    /// <summary>
    /// Update existing entity
    /// </summary>
    /// <param name="entity">Entity to update</param>
    public async Task UpdateAsync(FhirResource entity)
    {
        _context.FhirResources.Update(entity);
        await _context.SaveChangesAsync(CancellationToken.None);
    }

    /// <summary>
    /// Delete entity
    /// </summary>
    /// <param name="entity">Entity to delete</param>
    public async Task DeleteAsync(FhirResource entity)
    {
        entity.IsDeleted = true;
        await UpdateAsync(entity);
    }

    /// <summary>
    /// Check if entity exists
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>True if exists, false otherwise</returns>
    public async Task<bool> ExistsAsync(Guid id, string tenantId)
    {
        return await _context.FhirResources
            .AnyAsync(r => r.Id == id && r.TenantId == tenantId && !r.IsDeleted);
    }

    /// <summary>
    /// Get FHIR resource by type and ID
    /// </summary>
    /// <param name="resourceType">FHIR resource type</param>
    /// <param name="fhirId">FHIR resource ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>FHIR resource or null if not found</returns>
    public async Task<FhirResource?> GetByFhirIdAsync(string resourceType, string fhirId, string tenantId)
    {
        return await _context.FhirResources
            .Where(r => r.ResourceType == resourceType && r.FhirId == fhirId && r.TenantId == tenantId && !r.IsDeleted)
            .OrderByDescending(r => r.VersionId)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Get FHIR resources by type
    /// </summary>
    /// <param name="resourceType">FHIR resource type</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <param name="skip">Number of records to skip</param>
    /// <param name="take">Number of records to take</param>
    /// <returns>Collection of FHIR resources</returns>
    public async Task<IEnumerable<FhirResource>> GetByTypeAsync(string resourceType, string tenantId, int skip = 0, int take = 100)
    {
        return await _context.FhirResources
            .Where(r => r.ResourceType == resourceType && r.TenantId == tenantId && !r.IsDeleted)
            .OrderByDescending(r => r.LastUpdated)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    /// <summary>
    /// Search FHIR resources by parameters
    /// </summary>
    /// <param name="resourceType">FHIR resource type</param>
    /// <param name="searchParameters">Search parameters</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <param name="skip">Number of records to skip</param>
    /// <param name="take">Number of records to take</param>
    /// <returns>Collection of FHIR resources</returns>
    public async Task<IEnumerable<FhirResource>> SearchAsync(string resourceType, Dictionary<string, string> searchParameters, string tenantId, int skip = 0, int take = 100)
    {
        var query = _context.FhirResources
            .Where(r => r.ResourceType == resourceType && r.TenantId == tenantId && !r.IsDeleted);

        // Apply search parameters using JSONB queries
        foreach (var param in searchParameters)
        {
            var jsonPath = $"$.{param.Key}";
            query = query.Where(r => EF.Functions.JsonContains(r.SearchParameters, $"\"{param.Value}\""));
        }

        return await query
            .OrderByDescending(r => r.LastUpdated)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    /// <summary>
    /// Get FHIR resource history
    /// </summary>
    /// <param name="resourceType">FHIR resource type</param>
    /// <param name="fhirId">FHIR resource ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>Collection of FHIR resource versions</returns>
    public async Task<IEnumerable<FhirResource>> GetHistoryAsync(string resourceType, string fhirId, string tenantId)
    {
        return await _context.FhirResources
            .Where(r => r.ResourceType == resourceType && r.FhirId == fhirId && r.TenantId == tenantId && !r.IsDeleted)
            .OrderByDescending(r => r.VersionId)
            .ToListAsync();
    }

    /// <summary>
    /// Get FHIR resource by version
    /// </summary>
    /// <param name="resourceType">FHIR resource type</param>
    /// <param name="fhirId">FHIR resource ID</param>
    /// <param name="versionId">Version ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>FHIR resource or null if not found</returns>
    public async Task<FhirResource?> GetByVersionAsync(string resourceType, string fhirId, int versionId, string tenantId)
    {
        return await _context.FhirResources
            .FirstOrDefaultAsync(r => r.ResourceType == resourceType && r.FhirId == fhirId && r.VersionId == versionId && r.TenantId == tenantId && !r.IsDeleted);
    }

    /// <summary>
    /// Get FHIR resource by type and ID (alias for GetByFhirIdAsync)
    /// </summary>
    /// <param name="resourceType">FHIR resource type</param>
    /// <param name="fhirId">FHIR resource ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>FHIR resource or null if not found</returns>
    public async Task<FhirResource?> GetByTypeAndIdAsync(string resourceType, string fhirId, CancellationToken cancellationToken)
    {
        // For now, we'll use a default tenant ID - this should be enhanced to get tenant from context
        var tenantId = "default"; // This should be injected or retrieved from context
        return await GetByFhirIdAsync(resourceType, fhirId, tenantId);
    }
}
