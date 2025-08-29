using MediatR;
using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HealthTech.Application.FhirResources.Queries;

/// <summary>
/// Handler for GetFhirResourceHistoryQuery
/// </summary>
public class GetFhirResourceHistoryQueryHandler : IRequestHandler<GetFhirResourceHistoryQuery, GetFhirResourceHistoryResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Application database context</param>
    /// <param name="currentUserService">Current user service</param>
    public GetFhirResourceHistoryQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Handle the query
    /// </summary>
    /// <param name="request">Query request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response</returns>
    public async Task<GetFhirResourceHistoryResponse> Handle(GetFhirResourceHistoryQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.TenantId;

        // Get all versions of the resource
        var versions = await _context.FhirResources
            .Where(f => f.TenantId == tenantId &&
                       f.ResourceType == request.ResourceType &&
                       f.FhirId == request.FhirId)
            .OrderByDescending(f => f.VersionId)
            .Take(request.MaxVersions)
            .Select(f => new FhirResourceVersion
            {
                VersionId = f.VersionId,
                ResourceJson = f.ResourceJson,
                CreatedAt = f.CreatedAt,
                LastUpdated = f.LastUpdated ?? DateTime.UtcNow,
                IsCurrentVersion = f.Status == "active" && !f.IsDeleted,
                Operation = DetermineOperation(f)
            })
            .ToListAsync(cancellationToken);

        return new GetFhirResourceHistoryResponse
        {
            ResourceType = request.ResourceType,
            FhirId = request.FhirId,
            Versions = versions,
            TotalCount = versions.Count()
        };
    }

    private static string DetermineOperation(Domain.Entities.FhirResource resource)
    {
        if (resource.IsDeleted || resource.Status == "deleted")
            return "delete";
        
        if (resource.VersionId == 1)
            return "create";
        
        return "update";
    }
}
