using HealthTech.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace HealthTech.Infrastructure.Common.Services;

/// <summary>
/// JWT service implementation
/// </summary>
public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtService> _logger;
    private readonly IApplicationDbContext _context;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="configuration">Configuration</param>
    /// <param name="logger">Logger</param>
    /// <param name="context">Application database context</param>
    public JwtService(IConfiguration configuration, ILogger<JwtService> logger, IApplicationDbContext context)
    {
        _configuration = configuration;
        _logger = logger;
        _context = context;
        
        _secretKey = _configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT secret key not configured");
        _issuer = _configuration["JwtSettings:Issuer"] ?? "HealthTech.FHIR-AI";
        _audience = _configuration["JwtSettings:Audience"] ?? "HealthTech.FHIR-AI.Users";
        _expirationMinutes = int.TryParse(_configuration["JwtSettings:ExpirationMinutes"], out var exp) ? exp : 60;
    }

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
    public string GenerateToken(
        string userId,
        string username,
        string email,
        string role,
        string tenantId,
        IEnumerable<string> scopes,
        string? practitionerId = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Email, email),
            new("user_role", role),
            new("tenant_id", tenantId),
            new("scope", string.Join(" ", scopes))
        };

        if (!string.IsNullOrEmpty(practitionerId))
        {
            claims.Add(new Claim("practitioner_id", practitionerId));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        
        _logger.LogInformation("Generated JWT token for user {UserId}", userId);
        return tokenString;
    }

    /// <summary>
    /// Generate refresh token
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Refresh token</returns>
    public string GenerateRefreshToken(string userId)
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        
        var refreshToken = Convert.ToBase64String(randomNumber);
        
        _logger.LogInformation("Generated refresh token for user {UserId}", userId);
        return refreshToken;
    }

    /// <summary>
    /// Create and store refresh token for user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <param name="ipAddress">IP address</param>
    /// <param name="userAgent">User agent</param>
    /// <param name="expirationHours">Expiration time in hours (default: 24)</param>
    /// <returns>Refresh token</returns>
    public async Task<string> CreateRefreshTokenAsync(string userId, string tenantId, string? ipAddress = null, string? userAgent = null, int expirationHours = 24)
    {
        try
        {
            // Generate refresh token
            var refreshToken = GenerateRefreshToken(userId);

            // Create user session
            var userSession = new HealthTech.Domain.Entities.UserSession
            {
                UserId = Guid.Parse(userId),
                SessionToken = refreshToken, // Using refresh token as session token for simplicity
                RefreshToken = refreshToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(expirationHours),
                CreatedIpAddress = ipAddress,
                UserAgent = userAgent,
                TenantId = tenantId,
                IsRevoked = false,
                CreatedBy = userId, // Set CreatedBy to the user ID
                IsDeleted = false // Set IsDeleted to false
            };

            // Save to database
            _context.UserSessions.Add(userSession);
            await _context.SaveChangesAsync(CancellationToken.None);

            _logger.LogInformation("Created refresh token session for user {UserId} with expiration {Expiration}", userId, userSession.ExpiresAt);
            return refreshToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating refresh token for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Revoke refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token to revoke</param>
    /// <param name="reason">Reason for revocation</param>
    /// <returns>True if revoked successfully, false otherwise</returns>
    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken, string? reason = null)
    {
        try
        {
            var userSession = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken && !s.IsRevoked);

            if (userSession == null)
            {
                _logger.LogWarning("Refresh token not found or already revoked: {RefreshToken}", refreshToken);
                return false;
            }

            userSession.IsRevoked = true;
            userSession.RevokedAt = DateTime.UtcNow;
            userSession.RevocationReason = reason;

            await _context.SaveChangesAsync(CancellationToken.None);

            _logger.LogInformation("Revoked refresh token for user {UserId} with reason: {Reason}", userSession.UserId, reason ?? "No reason provided");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking refresh token: {RefreshToken}", refreshToken);
            return false;
        }
    }

    /// <summary>
    /// Validate JWT token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Token validation result</returns>
    public async Task<Application.Common.Interfaces.TokenValidationResult> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;

            var result = new Application.Common.Interfaces.TokenValidationResult
            {
                IsValid = true,
                UserId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Username = principal.FindFirst(ClaimTypes.Name)?.Value,
                Email = principal.FindFirst(ClaimTypes.Email)?.Value,
                Role = principal.FindFirst("user_role")?.Value,
                TenantId = principal.FindFirst("tenant_id")?.Value,
                Scopes = principal.FindFirst("scope")?.Value?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? new string[0],
                PractitionerId = principal.FindFirst("practitioner_id")?.Value,
                ExpiresAt = jwtToken.ValidTo
            };

            _logger.LogDebug("Token validation successful for user {UserId}", result.UserId);
            return result;
        }
        catch (SecurityTokenExpiredException ex)
        {
            _logger.LogWarning("Token expired: {Message}", ex.Message);
            return new Application.Common.Interfaces.TokenValidationResult
            {
                IsValid = false,
                ErrorMessage = "Token has expired"
            };
        }
        catch (SecurityTokenInvalidSignatureException ex)
        {
            _logger.LogWarning("Invalid token signature: {Message}", ex.Message);
            return new Application.Common.Interfaces.TokenValidationResult
            {
                IsValid = false,
                ErrorMessage = "Invalid token signature"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token validation failed");
            return new Application.Common.Interfaces.TokenValidationResult
            {
                IsValid = false,
                ErrorMessage = "Token validation failed"
            };
        }
    }

    /// <summary>
    /// Validate refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <returns>User ID if valid, null otherwise</returns>
    public async Task<string?> ValidateRefreshTokenAsync(string refreshToken)
    {
        try
        {
            // Basic validation
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("Refresh token validation failed: token is null or empty");
                return null;
            }

            // Find the user session with the given refresh token
            var userSession = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken && 
                                         !s.IsRevoked &&
                                         s.ExpiresAt > DateTime.UtcNow);

            if (userSession == null)
            {
                _logger.LogWarning("Refresh token validation failed: invalid, revoked, or expired token");
                return null;
            }

            // Additional security check: verify session is active
            if (!userSession.IsActive)
            {
                _logger.LogWarning("Refresh token validation failed: session is not active for user {UserId}", userSession.UserId);
                return null;
            }

            // Update last accessed timestamp
            userSession.LastAccessedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(CancellationToken.None);

            _logger.LogInformation("Refresh token validation successful for user {UserId}", userSession.UserId);
            return userSession.UserId.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating refresh token");
            return null;
        }
    }

    /// <summary>
    /// Get claims from token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Collection of claims</returns>
    public IEnumerable<Claim> GetClaimsFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get claims from token");
            return new List<Claim>();
        }
    }

    /// <summary>
    /// Get user ID from token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User ID or null</returns>
    public string? GetUserIdFromToken(string token)
    {
        var claims = GetClaimsFromToken(token);
        return claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }

    /// <summary>
    /// Get tenant ID from token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Tenant ID or null</returns>
    public string? GetTenantIdFromToken(string token)
    {
        var claims = GetClaimsFromToken(token);
        return claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value;
    }

    /// <summary>
    /// Get scopes from token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Collection of scopes</returns>
    public IEnumerable<string> GetScopesFromToken(string token)
    {
        var claims = GetClaimsFromToken(token);
        var scopeClaim = claims.FirstOrDefault(c => c.Type == "scope")?.Value;
        return scopeClaim?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
    }

    /// <summary>
    /// Check if token is expired
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>True if expired, false otherwise</returns>
    public bool IsTokenExpired(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.ValidTo < DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check token expiration");
            return true; // Assume expired if we can't read the token
        }
    }
}
