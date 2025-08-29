using MediatR;
using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Entities;
using HealthTech.Domain.Repositories;
using System.Text.Json;

namespace HealthTech.Application.FhirResources.Commands;

/// <summary>
/// Handler for DeleteFhirResourceCommand
/// </summary>
public class DeleteFhirResourceCommandHandler : IRequestHandler<DeleteFhirResourceCommand, DeleteFhirResourceResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IFhirResourceRepository _fhirResourceRepository;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Application database context</param>
    /// <param name="fhirResourceRepository">FHIR resource repository</param>
    /// <param name="currentUserService">Current user service</param>
    public DeleteFhirResourceCommandHandler(
        IApplicationDbContext context,
        IFhirResourceRepository fhirResourceRepository,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _fhirResourceRepository = fhirResourceRepository;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Handle the command
    /// </summary>
    /// <param name="request">Command request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response</returns>
    public async Task<DeleteFhirResourceResponse> Handle(DeleteFhirResourceCommand request, CancellationToken cancellationToken)
    {
        // Get existing resource
        var existingResource = await _fhirResourceRepository.GetByFhirIdAsync(
            request.ResourceType, 
            request.FhirId, 
            _currentUserService.TenantId ?? string.Empty);

        if (existingResource == null)
        {
            throw new InvalidOperationException($"Resource {request.ResourceType}/{request.FhirId} not found");
        }

        // Soft delete - mark as deleted
        existingResource.Status = "deleted";
        existingResource.LastUpdated = DateTime.UtcNow;
        
        existingResource.IsDeleted = true;
        existingResource.DeletedAt = DateTime.UtcNow;
        existingResource.DeletedBy = _currentUserService.UserId ?? "system";

        // Add audit trail
        var auditEvent = new Domain.Entities.AuditEvent
        {
            Id = Guid.NewGuid(),
            TenantId = _currentUserService.TenantId ?? string.Empty,
            EventType = "resource-delete",
            ResourceType = request.ResourceType,
            ResourceId = request.FhirId,
            UserId = _currentUserService.UserId ?? "system",
            CreatedAt = DateTime.UtcNow,
            EventData = JsonSerializer.Serialize(new
            {
                Version = existingResource.VersionId,
                DeletionReason = "Soft delete via API"
            })
        };

        _context.AuditEvents.Add(auditEvent);

        // Save changes
        await _context.SaveChangesAsync(cancellationToken);

        return new DeleteFhirResourceResponse
        {
            FhirId = request.FhirId,
            DeletedAt = DateTime.UtcNow,
            Success = true
        };
    }
}
