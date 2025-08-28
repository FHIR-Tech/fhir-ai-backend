using MediatR;
using FluentValidation;
using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Entities;

namespace HealthTech.Application.Authentication.Commands;

public record RefreshTokenCommand : IRequest<RefreshTokenResponse>
{
    public string RefreshToken { get; init; } = string.Empty;
}

public record RefreshTokenResponse
{
    public bool Success { get; init; }
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public string? ErrorMessage { get; init; }
}

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required");
    }
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;

    public RefreshTokenCommandHandler(
        IUserService userService,
        IJwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate refresh token and get user session
            var session = await _userService.ValidateRefreshTokenAsync(request.RefreshToken);
            if (session == null)
            {
                return new RefreshTokenResponse
                {
                    Success = false,
                    ErrorMessage = "Invalid or expired refresh token"
                };
            }

            // Get user and scopes
            var user = await _userService.GetUserByIdAsync(session.UserId);
            if (user == null)
            {
                return new RefreshTokenResponse
                {
                    Success = false,
                    ErrorMessage = "User not found"
                };
            }

            var scopes = await _userService.GetUserScopesAsync(user.Id);

            // Generate new tokens
            var newAccessToken = _jwtService.GenerateAccessToken(user, scopes);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            // Update session with new refresh token
            await _userService.UpdateUserSessionAsync(session.Id, newRefreshToken);

            return new RefreshTokenResponse
            {
                Success = true,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
        catch (Exception ex)
        {
            return new RefreshTokenResponse
            {
                Success = false,
                ErrorMessage = "An error occurred while refreshing token"
            };
        }
    }
}
