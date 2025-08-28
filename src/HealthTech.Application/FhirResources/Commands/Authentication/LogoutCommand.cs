using MediatR;
using FluentValidation;
using HealthTech.Application.Common.Interfaces;

namespace HealthTech.Application.FhirResources.Commands.Authentication;

/// <summary>
/// Command for user logout
/// </summary>
public record LogoutCommand : IRequest<LogoutResponse>
{
    public string SessionToken { get; init; } = string.Empty;
    public string? Reason { get; init; }
}

/// <summary>
/// Response for logout command
/// </summary>
public record LogoutResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// Validator for logout command
/// </summary>
public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.SessionToken)
            .NotEmpty().WithMessage("Session token is required");
    }
}

/// <summary>
/// Handler for logout command
/// </summary>
public class LogoutCommandHandler : IRequestHandler<LogoutCommand, LogoutResponse>
{
    private readonly IUserService _userService;

    public LogoutCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<LogoutResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Revoke the session
            var success = await _userService.RevokeSessionAsync(request.SessionToken, request.Reason);

            if (!success)
            {
                return new LogoutResponse
                {
                    Success = false,
                    ErrorMessage = "Session not found or already revoked"
                };
            }

            return new LogoutResponse
            {
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new LogoutResponse
            {
                Success = false,
                ErrorMessage = "An error occurred during logout. Please try again."
            };
        }
    }
}
