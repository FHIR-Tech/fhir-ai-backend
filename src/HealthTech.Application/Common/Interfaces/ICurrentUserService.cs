namespace HealthTech.Application.Common.Interfaces;

/// <summary>
/// Service for getting current user information
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Current user ID
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Current user display name
    /// </summary>
    string? UserDisplayName { get; }

    /// <summary>
    /// Current tenant ID
    /// </summary>
    string? TenantId { get; }

    /// <summary>
    /// User's IP address
    /// </summary>
    string? UserIpAddress { get; }

    /// <summary>
    /// User's FHIR scopes
    /// </summary>
    IEnumerable<string> Scopes { get; }

    /// <summary>
    /// Check if user has specific scope
    /// </summary>
    /// <param name="scope">Scope to check</param>
    /// <returns>True if user has scope, false otherwise</returns>
    bool HasScope(string scope);

    /// <summary>
    /// Check if user has any of the specified scopes
    /// </summary>
    /// <param name="scopes">Scopes to check</param>
    /// <returns>True if user has any scope, false otherwise</returns>
    bool HasAnyScope(params string[] scopes);
}
