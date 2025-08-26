using HealthTech.Domain.Entities;

namespace HealthTech.Application.Common.Interfaces;

/// <summary>
/// Service for logging audit events
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Log an audit event
    /// </summary>
    /// <param name="auditEvent">Audit event to log</param>
    /// <returns>Task</returns>
    Task LogEventAsync(AuditEvent auditEvent);

    /// <summary>
    /// Get audit events for a resource
    /// </summary>
    /// <param name="resourceType">Resource type</param>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <param name="skip">Number of records to skip</param>
    /// <param name="take">Number of records to take</param>
    /// <returns>Collection of audit events</returns>
    Task<IEnumerable<AuditEvent>> GetResourceAuditEventsAsync(string resourceType, string resourceId, string tenantId, int skip = 0, int take = 100);

    /// <summary>
    /// Get audit events for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <param name="skip">Number of records to skip</param>
    /// <param name="take">Number of records to take</param>
    /// <returns>Collection of audit events</returns>
    Task<IEnumerable<AuditEvent>> GetUserAuditEventsAsync(string userId, string tenantId, int skip = 0, int take = 100);
}
