using MediatR;
using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Entities;
using HealthTech.Application.Authentication.Commands.RefreshToken;
using HealthTech.Application.Authentication.DTOs;

namespace HealthTech.Application.Authentication.Commands.RefreshToken;

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
            var user = await _userService.GetUserByIdAsync(session.UserId, session.TenantId ?? "default");
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
            var newAccessToken = _jwtService.GenerateToken(
                user.Id.ToString(),
                user.Username,
                user.Email,
                user.Role.ToString(),
                session.TenantId ?? "default",
                scopes,
                user.PractitionerId
            );
            var newRefreshToken = _jwtService.GenerateRefreshToken(user.Id.ToString());

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
