using MediatR;
using Microsoft.AspNetCore.Mvc;
using HealthTech.Application.Authentication.Commands;
using HealthTech.Application.PatientAccess.Queries;
using HealthTech.Application.PatientAccess.Commands;
using HealthTech.Domain.Enums;

namespace HealthTech.API.Endpoints;

/// <summary>
/// Authentication endpoints
/// </summary>
public static class AuthenticationEndpoints
{
    /// <summary>
    /// Map authentication endpoints
    /// </summary>
    /// <param name="app">Web application</param>
    public static void MapAuthenticationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication")
            .WithOpenApi();

        // Login endpoint (no authentication required)
        group.MapPost("/login", Login)
            .WithName("Login")
            .WithSummary("User login")
            .WithDescription("Authenticate user and return JWT token")
            .Produces<LoginResponse>(200)
            .ProducesProblem(400)
            .ProducesProblem(401);

        // Refresh token endpoint (no authentication required)
        group.MapPost("/refresh", RefreshToken)
            .WithName("RefreshToken")
            .WithSummary("Refresh JWT token")
            .WithDescription("Refresh JWT token using refresh token")
            .Produces<RefreshTokenResponse>(200)
            .ProducesProblem(400)
            .ProducesProblem(401);

        // Logout endpoint (authentication required)
        group.MapPost("/logout", Logout)
            .WithName("Logout")
            .WithSummary("User logout")
            .WithDescription("Logout user and revoke session")
            .RequireAuthorization()
            .Produces<LogoutResponse>(200)
            .ProducesProblem(400)
            .ProducesProblem(401);

        // Patient access endpoints (authentication required)
        var patientAccessGroup = group.MapGroup("/patient-access")
            .RequireAuthorization()
            .WithTags("Patient Access");

        // Grant patient access
        patientAccessGroup.MapPost("/grant", GrantPatientAccess)
            .WithName("GrantPatientAccess")
            .WithSummary("Grant patient access")
            .WithDescription("Grant access to patient data for a user")
            .Produces<GrantPatientAccessResponse>(200)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(403);

        // Revoke patient access
        patientAccessGroup.MapPost("/revoke", RevokePatientAccess)
            .WithName("RevokePatientAccess")
            .WithSummary("Revoke patient access")
            .WithDescription("Revoke access to patient data for a user")
            .Produces<RevokePatientAccessResponse>(200)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(403);

        // Get patient access list
        patientAccessGroup.MapGet("/{patientId}", GetPatientAccess)
            .WithName("GetPatientAccess")
            .WithSummary("Get patient access list")
            .WithDescription("Get list of users with access to patient data")
            .Produces<GetPatientAccessResponse>(200)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(403);
    }

    /// <summary>
    /// Login endpoint
    /// </summary>
    /// <param name="command">Login command</param>
    /// <param name="mediator">Mediator</param>
    /// <param name="httpContext">HTTP context</param>
    /// <returns>Login response</returns>
    private static async Task<IResult> Login(
        [FromBody] LoginCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        try
        {
            var response = await mediator.Send(command);

            if (!response.Success)
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "Login failed",
                    Detail = response.ErrorMessage,
                    Status = 400
                });
            }

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Login error",
                detail: "An error occurred during login",
                statusCode: 500);
        }
    }

    /// <summary>
    /// Refresh token endpoint
    /// </summary>
    /// <param name="command">Refresh token command</param>
    /// <param name="mediator">Mediator</param>
    /// <returns>Refresh token response</returns>
    private static async Task<IResult> RefreshToken(
        [FromBody] RefreshTokenCommand command,
        [FromServices] IMediator mediator)
    {
        try
        {
            var response = await mediator.Send(command);

            if (!response.Success)
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "Token refresh failed",
                    Detail = response.ErrorMessage,
                    Status = 400
                });
            }

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Token refresh error",
                detail: "An error occurred while refreshing token",
                statusCode: 500);
        }
    }

    /// <summary>
    /// Logout endpoint
    /// </summary>
    /// <param name="command">Logout command</param>
    /// <param name="mediator">Mediator</param>
    /// <param name="httpContext">HTTP context</param>
    /// <returns>Logout response</returns>
    private static async Task<IResult> Logout(
        [FromBody] LogoutCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        try
        {
            // Get session token from Authorization header
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var authHeader = httpContext.Request.Headers.Authorization.ToString();
                if (authHeader.StartsWith("Bearer "))
                {
                    command = command with
                    {
                        SessionToken = authHeader.Substring("Bearer ".Length)
                    };
                }
            }

            var response = await mediator.Send(command);

            if (!response.Success)
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "Logout failed",
                    Detail = response.ErrorMessage,
                    Status = 400
                });
            }

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Logout error",
                detail: "An error occurred during logout",
                statusCode: 500);
        }
    }

    /// <summary>
    /// Grant patient access endpoint
    /// </summary>
    /// <param name="command">Grant patient access command</param>
    /// <param name="mediator">Mediator</param>
    /// <returns>Grant patient access response</returns>
    private static async Task<IResult> GrantPatientAccess(
        [FromBody] GrantPatientAccessCommand command,
        [FromServices] IMediator mediator)
    {
        try
        {
            var response = await mediator.Send(command);

            if (!response.Success)
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "Grant patient access failed",
                    Detail = response.ErrorMessage,
                    Status = 400
                });
            }

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Grant patient access error",
                detail: "An error occurred while granting patient access",
                statusCode: 500);
        }
    }

    /// <summary>
    /// Revoke patient access endpoint
    /// </summary>
    /// <param name="command">Revoke patient access command</param>
    /// <param name="mediator">Mediator</param>
    /// <returns>Revoke patient access response</returns>
    private static async Task<IResult> RevokePatientAccess(
        [FromBody] RevokePatientAccessCommand command,
        [FromServices] IMediator mediator)
    {
        try
        {
            var response = await mediator.Send(command);

            if (!response.Success)
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "Revoke patient access failed",
                    Detail = response.ErrorMessage,
                    Status = 400
                });
            }

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Revoke patient access error",
                detail: "An error occurred while revoking patient access",
                statusCode: 500);
        }
    }

    /// <summary>
    /// Get patient access endpoint
    /// </summary>
    /// <param name="patientId">Patient ID</param>
    /// <param name="mediator">Mediator</param>
    /// <param name="userId">User ID filter</param>
    /// <param name="accessLevel">Access level filter</param>
    /// <param name="isActive">Active status filter</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Get patient access response</returns>
    private static async Task<IResult> GetPatientAccess(
        string patientId,
        [FromServices] IMediator mediator,
        [FromQuery] string? userId = null,
        [FromQuery] PatientAccessLevel? accessLevel = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new GetPatientAccessQuery
            {
                PatientId = patientId,
                UserId = userId,
                AccessLevel = accessLevel,
                IsActive = isActive,
                Page = page,
                PageSize = pageSize
            };

            var response = await mediator.Send(query);

            if (!response.Success)
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "Get patient access failed",
                    Detail = response.ErrorMessage,
                    Status = 400
                });
            }

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Get patient access error",
                detail: "An error occurred while retrieving patient access",
                statusCode: 500);
        }
    }
}
