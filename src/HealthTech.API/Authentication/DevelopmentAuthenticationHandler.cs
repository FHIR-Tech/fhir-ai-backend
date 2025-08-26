using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace HealthTech.API.Authentication;

/// <summary>
/// Development authentication handler for testing purposes
/// </summary>
public class DevelopmentAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    /// <summary>
    /// Constructor
    /// </summary>
    public DevelopmentAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    /// <summary>
    /// Handle authentication
    /// </summary>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // For development, always authenticate successfully
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "development-user"),
            new Claim(ClaimTypes.NameIdentifier, "dev-user-001"),
            new Claim("tenant_id", "demo-tenant"),
            new Claim("scope", "user/* patient/* system/*")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    /// <summary>
    /// Handle challenge
    /// </summary>
    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Headers["WWW-Authenticate"] = "Development";
        return base.HandleChallengeAsync(properties);
    }
}
