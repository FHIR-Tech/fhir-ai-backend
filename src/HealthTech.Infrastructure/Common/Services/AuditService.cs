using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthTech.Infrastructure.Common.Services;

/// <summary>
/// Audit service implementation
/// </summary>
public class AuditService : IAuditService
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Application database context</param>
    public AuditService(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Log an audit event
    /// </summary>
    /// <param name="auditEvent">Audit event to log</param>
    /// <returns>Task</returns>
    public async Task LogEventAsync(AuditEvent auditEvent)
    {
        try
        {
            _context.AuditEvents.Add(auditEvent);
            await _context.SaveChangesAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            // Log the error but don't throw to avoid breaking the main flow
            // In production, you might want to use a separate logging service
            Console.WriteLine($"Failed to log audit event: {ex.Message}");
        }
    }

    /// <summary>
    /// Get audit events for a resource
    /// </summary>
    /// <param name="resourceType">Resource type</param>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <param name="skip">Number of records to skip</param>
    /// <param name="take">Number of records to take</param>
    /// <returns>Collection of audit events</returns>
    public async Task<IEnumerable<AuditEvent>> GetResourceAuditEventsAsync(string resourceType, string resourceId, string tenantId, int skip = 0, int take = 100)
    {
        return await _context.AuditEvents
            .Where(e => e.ResourceType == resourceType && 
                       e.ResourceId == resourceId && 
                       e.TenantId == tenantId &&
                       !e.IsDeleted)
            .OrderByDescending(e => e.EventTime)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    /// <summary>
    /// Get audit events for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <param name="skip">Number of records to skip</param>
    /// <param name="take">Number of records to take</param>
    /// <returns>Collection of audit events</returns>
    public async Task<IEnumerable<AuditEvent>> GetUserAuditEventsAsync(string userId, string tenantId, int skip = 0, int take = 100)
    {
        return await _context.AuditEvents
            .Where(e => e.UserId == userId && 
                       e.TenantId == tenantId &&
                       !e.IsDeleted)
            .OrderByDescending(e => e.EventTime)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }
}
