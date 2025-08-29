using MediatR;
using HealthTech.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthTech.Application.FhirResources.Queries.GetFhirResource;

/// <summary>
/// Handler for GetFhirResourceQuery
/// </summary>
public class GetFhirResourceQueryHandler : IRequestHandler<GetFhirResourceQuery, GetFhirResourceResponse?>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Application database context</param>
    /// <param name="currentUserService">Current user service</param>
    public GetFhirResourceQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Handle the query
    /// </summary>
    /// <param name="request">Query request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response or null if not found</returns>
    public async Task<GetFhirResourceResponse?> Handle(GetFhirResourceQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.TenantId ?? string.Empty;

        var resource = await _context.FhirResources
            .Where(r => r.TenantId == tenantId && 
                       r.ResourceType == request.ResourceType && 
                       r.FhirId == request.FhirId && 
                       r.Status == "active")
            .Select(r => new GetFhirResourceResponse
            {
                FhirId = r.FhirId,
                VersionId = r.VersionId,
                ResourceJson = r.ResourceJson,
                LastUpdated = r.LastUpdated
            })
            .FirstOrDefaultAsync(cancellationToken);

        return resource;
    }
}
