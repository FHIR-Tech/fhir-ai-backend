using MediatR;
using FluentValidation;
using HealthTech.Application.Common.Interfaces;

namespace HealthTech.Application.FhirResources.Commands.Authentication;

/// <summary>
/// Command for refreshing JWT token
/// </summary>
public record RefreshTokenCommand : IRequest<RefreshTokenResponse>
{
    public string RefreshToken { get; init; } = string.Empty;
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
}

/// <summary>
/// Response for refresh token command
/// </summary>
public record RefreshTokenResponse
{
    public bool Success { get; init; }
    public string? Token { get; init; }
    public string? RefreshToken { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// Validator for refresh token command
/// </summary>
public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required");
    }
}

/// <summary>
/// Handler for refresh token command
/// </summary>
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
            // Validate refresh token
            var session = await _userService.ValidateSessionAsync(request.RefreshToken);

            if (session == null)
            {
                return new RefreshTokenResponse
                {
                    Success = false,
                    ErrorMessage = "Invalid or expired refresh token"
                };
            }

            // Check if session is still active
            if (!session.IsActive || session.ExpiresAt < DateTime.UtcNow)
            {
                return new RefreshTokenResponse
                {
                    Success = false,
                    ErrorMessage = "Session has expired"
                };
            }

            // Get user information
            var user = await _userService.GetByIdAsync(session.UserId);

            if (user == null)
            {
                return new RefreshTokenResponse
                {
                    Success = false,
                    ErrorMessage = "User not found"
                };
            }

            // Check if user account is still active
            if (user.Status != "Active")
            {
                return new RefreshTokenResponse
                {
                    Success = false,
                    ErrorMessage = "User account is not active"
                };
            }

            // Generate new JWT token
            var newToken = await _jwtService.GenerateTokenAsync(user.Id, user.Username, user.Role);

            // Generate new refresh token
            var newRefreshToken = await _jwtService.GenerateRefreshTokenAsync();

            // Update session with new tokens
            await _userService.UpdateSessionAsync(
                session.Id,
                newToken,
                newRefreshToken,
                request.IpAddress,
                request.UserAgent);

            return new RefreshTokenResponse
            {
                Success = true,
                Token = newToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1) // Token expires in 1 hour
            };
        }
        catch (Exception ex)
        {
            return new RefreshTokenResponse
            {
                Success = false,
                ErrorMessage = "An error occurred while refreshing token. Please try again."
            };
        }
    }
}
