using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace HealthTech.API.Authentication;

/// <summary>
/// SMART on FHIR authentication handler
/// </summary>
public class SmartOnFhirAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<SmartOnFhirAuthenticationHandler> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="options">Authentication scheme options</param>
    /// <param name="logger">Logger factory</param>
    /// <param name="encoder">URL encoder</param>
    /// <param name="clock">System clock</param>
    /// <param name="userService">User service</param>
    /// <param name="jwtService">JWT service</param>
    /// <param name="currentUserService">Current user service</param>
    public SmartOnFhirAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IUserService userService,
        IJwtService jwtService,
        ICurrentUserService currentUserService)
        : base(options, logger, encoder, clock)
    {
        _userService = userService;
        _jwtService = jwtService;
        _currentUserService = currentUserService;
        _logger = logger.CreateLogger<SmartOnFhirAuthenticationHandler>();
    }

    /// <summary>
    /// Handle authentication
    /// </summary>
    /// <returns>Authentication result</returns>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            var token = GetTokenFromRequest();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogDebug("No token provided in request");
                return AuthenticateResult.Fail("No token provided");
            }

            var claims = await ValidateTokenAndGetClaims(token);
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            _logger.LogDebug("Authentication successful for user {UserId}", claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            return AuthenticateResult.Success(ticket);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token validation failed");
            return AuthenticateResult.Fail($"Token validation failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Handle challenge
    /// </summary>
    /// <param name="properties">Authentication properties</param>
    /// <returns>Task</returns>
    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Headers["WWW-Authenticate"] = "Bearer";
        return base.HandleChallengeAsync(properties);
    }

    /// <summary>
    /// Get token from request
    /// </summary>
    /// <returns>Token string or null</returns>
    private string? GetTokenFromRequest()
    {
        // Check Authorization header
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }

        // Check for API key in development
        var apiKey = Request.Headers["X-API-Key"].FirstOrDefault();
        if (!string.IsNullOrEmpty(apiKey) && apiKey == "test-key")
        {
            return "development-token";
        }

        return null;
    }

    /// <summary>
    /// Validate token and get claims
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Collection of claims</returns>
    private async Task<IEnumerable<Claim>> ValidateTokenAndGetClaims(string token)
    {
        // Handle development token
        if (token == "development-token")
        {
            return GetDevelopmentClaims();
        }

        // Validate JWT token
        var tokenValidationResult = await _jwtService.ValidateTokenAsync(token);
        if (!tokenValidationResult.IsValid)
        {
            throw new UnauthorizedAccessException($"Token validation failed: {tokenValidationResult.ErrorMessage}");
        }

        // Get user information from database
        if (!Guid.TryParse(tokenValidationResult.UserId, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user ID in token");
        }

        var user = await _userService.GetUserByIdAsync(userId, tokenValidationResult.TenantId ?? "default");
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        // Check if user is active
        if (user.Status != UserStatus.Active)
        {
            throw new UnauthorizedAccessException("User account is not active");
        }

        // Get user scopes
        var userScopes = await _userService.GetUserScopesAsync(userId);
        var scopes = userScopes.ToList();

        // Add default scopes based on user role
        if (!scopes.Any())
        {
            scopes.AddRange(GetDefaultScopesForRole(user.Role));
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.Email, user.Email),
            new("user_role", user.Role.ToString()),
            new("tenant_id", user.TenantId),
            new("scope", string.Join(" ", scopes))
        };

        if (!string.IsNullOrEmpty(user.PractitionerId))
        {
            claims.Add(new Claim("practitioner_id", user.PractitionerId));
        }

        return claims;
    }

    /// <summary>
    /// Get development claims for testing
    /// </summary>
    /// <returns>Development claims</returns>
    private static IEnumerable<Claim> GetDevelopmentClaims()
    {
        return new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "dev-user-001"),
            new(ClaimTypes.Name, "Development User"),
            new(ClaimTypes.Email, "dev@healthtech.com"),
            new("user_role", "SystemAdministrator"),
            new("tenant_id", "demo-tenant"),
            new("scope", "system/* user/* patient/*"),
            new("practitioner_id", "dev-practitioner-001")
        };
    }

    /// <summary>
    /// Get default scopes for user role
    /// </summary>
    /// <param name="role">User role</param>
    /// <returns>Default scopes</returns>
    private static IEnumerable<string> GetDefaultScopesForRole(UserRole role)
    {
        return role switch
        {
            UserRole.SystemAdministrator => new[] { "system/*", "user/*", "patient/*" },
            UserRole.HealthcareProvider => new[] { "user/*", "patient/*" },
            UserRole.Nurse => new[] { "user/*", "patient/*" },
            UserRole.Patient => new[] { "patient/*" },
            UserRole.FamilyMember => new[] { "patient/*" },
            UserRole.Researcher => new[] { "user/*" },
            UserRole.ITSupport => new[] { "system/*" },
            UserRole.ReadOnlyUser => new[] { "user/*" },
            _ => new[] { "user/*" }
        };
    }
}
