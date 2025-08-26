using MediatR;
using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Entities;
using HealthTech.Domain.Repositories;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace HealthTech.Application.FhirResources.Commands.UpdateFhirResource;

/// <summary>
/// Handler for UpdateFhirResourceCommand
/// </summary>
public class UpdateFhirResourceCommandHandler : IRequestHandler<UpdateFhirResourceCommand, UpdateFhirResourceResponse>
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
    public UpdateFhirResourceCommandHandler(
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
    public async Task<UpdateFhirResourceResponse> Handle(UpdateFhirResourceCommand request, CancellationToken cancellationToken)
    {
        // Parse FHIR resource
        var parser = new FhirJsonParser();
        var resource = parser.Parse<Resource>(request.ResourceJson);

        // Get existing resource
        var existingResource = await _fhirResourceRepository.GetByFhirIdAsync(
            request.ResourceType, 
            request.FhirId, 
            _currentUserService.TenantId ?? string.Empty);

        if (existingResource == null)
        {
            throw new InvalidOperationException($"Resource {request.ResourceType}/{request.FhirId} not found");
        }

        // Create new version
        existingResource.VersionId++;
        existingResource.ResourceJson = request.ResourceJson;
        existingResource.LastUpdated = DateTime.UtcNow;
        existingResource.ModifiedBy = _currentUserService.UserId ?? "system";
        existingResource.ModifiedAt = DateTime.UtcNow;
        existingResource.SearchParameters = ExtractSearchParameters(resource);
        existingResource.Tags = ExtractTags(resource);
        existingResource.SecurityLabels = ExtractSecurityLabels(resource);

        // Add audit trail
        var auditEvent = new Domain.Entities.AuditEvent
        {
            Id = Guid.NewGuid(),
            TenantId = _currentUserService.TenantId ?? string.Empty,
            EventType = "resource-update",
            ResourceType = request.ResourceType,
            ResourceId = request.FhirId,
            UserId = _currentUserService.UserId ?? "system",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId ?? "system",
            EventData = JsonSerializer.Serialize(new
            {
                PreviousVersion = existingResource.VersionId - 1,
                NewVersion = existingResource.VersionId,
                Changes = "Resource updated"
            })
        };

        _context.AuditEvents.Add(auditEvent);

        // Save changes
        await _context.SaveChangesAsync(cancellationToken);

        return new UpdateFhirResourceResponse
        {
            FhirId = request.FhirId,
            VersionId = existingResource.VersionId,
            ResourceJson = request.ResourceJson
        };
    }

    private static string? ExtractSearchParameters(Resource resource)
    {
        // Extract common search parameters from FHIR resource
        var parameters = new Dictionary<string, object>();

        if (resource is Patient patient)
        {
            if (patient.Identifier?.Any() == true)
                parameters["identifier"] = patient.Identifier.First().Value;
            if (patient.Name?.Any() == true)
                parameters["name"] = patient.Name.First().Text ?? patient.Name.First().Given?.FirstOrDefault();
        }
        else if (resource is Observation observation)
        {
            if (observation.Code?.Coding?.Any() == true)
                parameters["code"] = observation.Code.Coding.First().Code;
            if (observation.Subject?.Reference != null)
                parameters["subject"] = observation.Subject.Reference;
        }

        return parameters.Any() ? JsonSerializer.Serialize(parameters) : null;
    }

    private static string? ExtractTags(Resource resource)
    {
        if (resource.Meta?.Tag?.Any() != true)
            return null;

        var tags = resource.Meta.Tag.Select(t => new { t.Code, t.Display, t.System }).ToList();
        return JsonSerializer.Serialize(tags);
    }

    private static string? ExtractSecurityLabels(Resource resource)
    {
        if (resource.Meta?.Security?.Any() != true)
            return null;

        var labels = resource.Meta.Security.Select(s => new { s.Code, s.Display, s.System }).ToList();
        return JsonSerializer.Serialize(labels);
    }
}
