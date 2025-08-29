using MediatR;
using HealthTech.Application.Common.Interfaces;
using HealthTech.Application.Authentication.Commands.Logout;
using HealthTech.Application.Authentication.DTOs;

namespace HealthTech.Application.Authentication.Commands.Logout;

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
                IsSuccess = success,
                Message = success ? "Logout successful" : "Session not found or already invalidated",
                RequestId = request.RequestId
            };
        }
        catch (Exception ex)
        {
            return new LogoutResponse
            {
                IsSuccess = false,
                Message = "An error occurred during logout",
                RequestId = request.RequestId
            };
        }
    }
}
