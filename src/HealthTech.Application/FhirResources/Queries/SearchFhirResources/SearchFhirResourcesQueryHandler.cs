using MediatR;
using HealthTech.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthTech.Application.FhirResources.Queries.SearchFhirResources;

/// <summary>
/// Handler for SearchFhirResourcesQuery
/// </summary>
public class SearchFhirResourcesQueryHandler : IRequestHandler<SearchFhirResourcesQuery, SearchFhirResourcesResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Application database context</param>
    /// <param name="currentUserService">Current user service</param>
    public SearchFhirResourcesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
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
    public async Task<SearchFhirResourcesResponse> Handle(SearchFhirResourcesQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.TenantId ?? string.Empty;

        // Build query
        var query = _context.FhirResources
            .Where(r => r.TenantId == tenantId && r.ResourceType == request.ResourceType && r.Status == "active");

        // Apply search parameters if provided
        if (request.SearchParameters.Any())
        {
            foreach (var param in request.SearchParameters)
            {
                switch (param.Key.ToLower())
                {
                    case "identifier":
                        query = query.Where(r => r.SearchParameters.Contains(param.Value));
                        break;
                    case "name":
                        query = query.Where(r => r.SearchParameters.Contains(param.Value));
                        break;
                    case "code":
                        query = query.Where(r => r.SearchParameters.Contains(param.Value));
                        break;
                    case "subject":
                        query = query.Where(r => r.ResourceJson.Contains(param.Value));
                        break;
                    // Add more search parameters as needed
                }
            }
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination and ordering
        var resources = await query
            .OrderByDescending(r => r.LastUpdated)
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(r => new SearchFhirResourcesResponse.FhirResourceDto
            {
                FhirId = r.FhirId,
                VersionId = r.VersionId,
                ResourceJson = r.ResourceJson,
                LastUpdated = r.LastUpdated
            })
            .ToListAsync(cancellationToken);

        return new SearchFhirResourcesResponse
        {
            Resources = resources,
            TotalCount = totalCount
        };
    }
}
