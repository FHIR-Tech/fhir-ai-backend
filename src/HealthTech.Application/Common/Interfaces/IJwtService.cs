using System.Security.Claims;

namespace HealthTech.Application.Common.Interfaces;

/// <summary>
/// Service for JWT token management
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generate JWT token for user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="username">Username</param>
    /// <param name="email">Email</param>
    /// <param name="role">User role</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <param name="scopes">User scopes</param>
    /// <param name="practitionerId">Practitioner ID (optional)</param>
    /// <returns>JWT token</returns>
    string GenerateToken(
        string userId,
        string username,
        string email,
        string role,
        string tenantId,
        IEnumerable<string> scopes,
        string? practitionerId = null);

    /// <summary>
    /// Generate refresh token
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Refresh token</returns>
    string GenerateRefreshToken(string userId);

    /// <summary>
    /// Validate JWT token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Token validation result</returns>
    Task<TokenValidationResult> ValidateTokenAsync(string token);

    /// <summary>
    /// Validate refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <returns>User ID if valid, null otherwise</returns>
    Task<string?> ValidateRefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Get claims from token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Collection of claims</returns>
    IEnumerable<Claim> GetClaimsFromToken(string token);

    /// <summary>
    /// Get user ID from token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User ID or null</returns>
    string? GetUserIdFromToken(string token);

    /// <summary>
    /// Get tenant ID from token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Tenant ID or null</returns>
    string? GetTenantIdFromToken(string token);

    /// <summary>
    /// Get scopes from token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Collection of scopes</returns>
    IEnumerable<string> GetScopesFromToken(string token);

    /// <summary>
    /// Check if token is expired
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>True if expired, false otherwise</returns>
    bool IsTokenExpired(string token);
}

/// <summary>
/// Result of token validation
/// </summary>
public class TokenValidationResult
{
    /// <summary>
    /// Whether token is valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// User ID from token
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Username from token
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Email from token
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Role from token
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// Tenant ID from token
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Scopes from token
    /// </summary>
    public IEnumerable<string> Scopes { get; set; } = new List<string>();

    /// <summary>
    /// Practitioner ID from token
    /// </summary>
    public string? PractitionerId { get; set; }

    /// <summary>
    /// Error message if validation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Token expiration time
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
}
