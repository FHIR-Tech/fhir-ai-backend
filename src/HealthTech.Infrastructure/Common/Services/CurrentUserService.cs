using HealthTech.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;

namespace HealthTech.Infrastructure.Common.Services;

/// <summary>
/// Current user service implementation
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="httpContextAccessor">HTTP context accessor</param>
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Current user ID
    /// </summary>
    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value
        ?? _httpContextAccessor.HttpContext?.User?.FindFirst("user_id")?.Value;

    /// <summary>
    /// Current user display name
    /// </summary>
    public string? UserDisplayName => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value
        ?? _httpContextAccessor.HttpContext?.User?.FindFirst("name")?.Value
        ?? _httpContextAccessor.HttpContext?.User?.FindFirst("preferred_username")?.Value;

    /// <summary>
    /// Current tenant ID
    /// </summary>
    public string? TenantId => _httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id")?.Value
        ?? _httpContextAccessor.HttpContext?.User?.FindFirst("org_id")?.Value
        ?? _httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-ID"].FirstOrDefault()
        ?? "default"; // Fallback for development

    /// <summary>
    /// User's IP address
    /// </summary>
    public string? UserIpAddress => _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
        ?? _httpContextAccessor.HttpContext?.Request.Headers["X-Forwarded-For"].FirstOrDefault()
        ?? _httpContextAccessor.HttpContext?.Request.Headers["X-Real-IP"].FirstOrDefault();

    /// <summary>
    /// User's FHIR scopes
    /// </summary>
    public IEnumerable<string> Scopes
    {
        get
        {
            var scopeClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("scope")?.Value
                ?? _httpContextAccessor.HttpContext?.User?.FindFirst("scopes")?.Value
                ?? _httpContextAccessor.HttpContext?.Request.Headers["X-FHIR-Scopes"].FirstOrDefault();

            if (string.IsNullOrEmpty(scopeClaim))
            {
                // Development fallback - provide full access
                return new[] { "system/*", "user/*", "patient/*" };
            }

            return scopeClaim.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }
    }

    /// <summary>
    /// Check if user has specific scope
    /// </summary>
    /// <param name="scope">Scope to check</param>
    /// <returns>True if user has scope, false otherwise</returns>
    public bool HasScope(string scope)
    {
        if (string.IsNullOrEmpty(scope))
            return false;

        var userScopes = Scopes.ToList();
        
        // Check for exact match
        if (userScopes.Contains(scope))
            return true;

        // Check for wildcard patterns
        foreach (var userScope in userScopes)
        {
            if (IsScopeMatch(userScope, scope))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Check if user has any of the specified scopes
    /// </summary>
    /// <param name="scopes">Scopes to check</param>
    /// <returns>True if user has any scope, false otherwise</returns>
    public bool HasAnyScope(params string[] scopes)
    {
        return scopes.Any(HasScope);
    }

    /// <summary>
    /// Check if a user scope matches a required scope (handles wildcards)
    /// </summary>
    /// <param name="userScope">User's scope</param>
    /// <param name="requiredScope">Required scope</param>
    /// <returns>True if scope matches</returns>
    private static bool IsScopeMatch(string userScope, string requiredScope)
    {
        // Handle wildcard patterns like "patient/*" or "user/Patient.read"
        var userParts = userScope.Split('.');
        var requiredParts = requiredScope.Split('.');

        if (userParts.Length != requiredParts.Length)
            return false;

        for (int i = 0; i < userParts.Length; i++)
        {
            if (userParts[i] == "*" || requiredParts[i] == "*")
                continue;

            if (userParts[i] != requiredParts[i])
                return false;
        }

        return true;
    }
}
