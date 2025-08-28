using MediatR;
using FluentValidation;
using HealthTech.Application.Common.Interfaces;

namespace HealthTech.Application.Authentication.Commands;

public record LogoutCommand : IRequest<LogoutResponse>
{
    public string SessionToken { get; init; } = string.Empty;
}

public record LogoutResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.SessionToken)
            .NotEmpty().WithMessage("Session token is required");
    }
}

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
            // Invalidate user session
            var success = await _userService.InvalidateUserSessionAsync(request.SessionToken);
            
            return new LogoutResponse
            {
                Success = success,
                ErrorMessage = success ? null : "Session not found or already invalidated"
            };
        }
        catch (Exception ex)
        {
            return new LogoutResponse
            {
                Success = false,
                ErrorMessage = "An error occurred during logout"
            };
        }
    }
}
