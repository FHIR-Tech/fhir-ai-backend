using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Entities;

namespace HealthTech.API.Middleware;

/// <summary>
/// Middleware for logging audit events
/// </summary>
public class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLoggingMiddleware> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="next">Next middleware</param>
    /// <param name="logger">Logger</param>
    public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invoke middleware
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>Task</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        var originalBodyStream = context.Response.Body;

        try
        {
            await _next(context);
        }
        finally
        {
            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;

            // Log audit event if it's a FHIR endpoint
            if (context.Request.Path.StartsWithSegments("/fhir"))
            {
                await LogAuditEventAsync(context, startTime, endTime, duration);
            }
        }
    }

    private async Task LogAuditEventAsync(HttpContext context, DateTime startTime, DateTime endTime, TimeSpan duration)
    {
        try
        {
            var auditService = context.RequestServices.GetService<IAuditService>();
            if (auditService != null)
            {
                var auditEvent = new AuditEvent
                {
                    Id = Guid.NewGuid(),
                    EventType = "API_ACCESS",
                    EventSubtype = context.Request.Method,
                    Action = GetActionFromMethod(context.Request.Method),
                    Outcome = context.Response.StatusCode < 400 ? 0 : 8, // 0=success, 8=major
                    Description = $"{context.Request.Method} {context.Request.Path}",
                    UserId = context.User?.Identity?.Name ?? "anonymous",
                    UserDisplayName = context.User?.Identity?.Name ?? "Anonymous User",
                    UserIpAddress = context.Connection.RemoteIpAddress?.ToString(),
                    ResourceType = GetResourceTypeFromPath(context.Request.Path),
                    ResourceId = GetResourceIdFromPath(context.Request.Path),
                    EventTime = startTime,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = context.User?.Identity?.Name ?? "system"
                };

                await auditService.LogEventAsync(auditEvent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log audit event");
        }
    }

    private static string GetActionFromMethod(string method)
    {
        return method.ToUpper() switch
        {
            "GET" => "R",
            "POST" => "C",
            "PUT" => "U",
            "DELETE" => "D",
            "PATCH" => "U",
            _ => "E"
        };
    }

    private static string? GetResourceTypeFromPath(PathString path)
    {
        var segments = path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return segments?.Length >= 2 ? segments[1] : null;
    }

    private static string? GetResourceIdFromPath(PathString path)
    {
        var segments = path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return segments?.Length >= 3 ? segments[2] : null;
    }
}
