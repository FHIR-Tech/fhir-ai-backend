using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthTech.API.Endpoints;

public static class DebugEndpoints
{
    public static void MapDebugEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/debug")
            .WithTags("Debug")
            .WithOpenApi();

        // Test user lookup
        group.MapGet("/user/{username}", async (
            string username,
            [FromServices] IUserService userService) =>
        {
            try
            {
                var user = await userService.GetUserByUsernameAsync(username, "default");
                if (user == null)
                {
                    return Results.NotFound($"User '{username}' not found");
                }

                return Results.Ok(new
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    Status = user.Status,
                    TenantId = user.TenantId,
                    IsDeleted = user.IsDeleted
                });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error looking up user: {ex.Message}");
            }
        })
        .WithName("DebugUserLookup")
        .WithSummary("Debug user lookup");

        // Test password verification
        group.MapPost("/verify-password", async (
            [FromBody] PasswordVerificationRequest request,
            [FromServices] IUserService userService) =>
        {
            try
            {
                var user = await userService.GetUserByUsernameAsync(request.Username, request.TenantId ?? "default");
                if (user == null)
                {
                    return Results.NotFound($"User '{request.Username}' not found");
                }

                // Use reflection to access private method for testing
                var method = typeof(HealthTech.Infrastructure.Common.Services.UserService)
                    .GetMethod("VerifyPassword", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                
                if (method == null)
                {
                    return Results.Problem("Could not access password verification method");
                }

                var isValid = (bool)method.Invoke(null, new object[] { request.Password, user.PasswordHash })!;

                return Results.Ok(new
                {
                    Username = user.Username,
                    PasswordHash = user.PasswordHash,
                    IsValid = isValid
                });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error verifying password: {ex.Message}");
            }
        })
        .WithName("DebugPasswordVerification")
        .WithSummary("Debug password verification");

        // Test user scopes
        group.MapGet("/user-scopes/{userId}", async (
            Guid userId,
            [FromServices] IUserService userService) =>
        {
            try
            {
                var scopes = await userService.GetUserScopesAsync(userId);
                return Results.Ok(new
                {
                    UserId = userId,
                    Scopes = scopes.ToList(),
                    Count = scopes.Count()
                });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error getting user scopes: {ex.Message}");
            }
        })
        .WithName("DebugUserScopes")
        .WithSummary("Debug user scopes");

        // Test JWT token generation
        group.MapPost("/generate-token", async (
            [FromBody] TokenGenerationRequest request,
            [FromServices] IJwtService jwtService) =>
        {
            try
            {
                var token = jwtService.GenerateToken(
                    request.UserId,
                    request.Username,
                    request.Email,
                    request.Role,
                    request.TenantId,
                    request.Scopes,
                    request.PractitionerId);

                return Results.Ok(new
                {
                    Token = token,
                    Length = token.Length
                });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error generating token: {ex.Message}");
            }
        })
        .WithName("DebugTokenGeneration")
        .WithSummary("Debug JWT token generation");

        // Test refresh token creation
        group.MapPost("/create-refresh-token", async (
            [FromBody] RefreshTokenRequest request,
            [FromServices] IJwtService jwtService) =>
        {
            try
            {
                var refreshToken = await jwtService.CreateRefreshTokenAsync(
                    request.UserId,
                    request.TenantId,
                    request.IpAddress,
                    request.UserAgent);

                return Results.Ok(new
                {
                    RefreshToken = refreshToken,
                    Length = refreshToken.Length
                });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error creating refresh token: {ex.Message}");
            }
        })
        .WithName("DebugRefreshTokenCreation")
        .WithSummary("Debug refresh token creation");
    }
}

public record PasswordVerificationRequest(string Username, string Password, string? TenantId);
public record TokenGenerationRequest(string UserId, string Username, string Email, string Role, string TenantId, IEnumerable<string> Scopes, string? PractitionerId);
public record RefreshTokenRequest(string UserId, string TenantId, string? IpAddress, string? UserAgent);

