using MediatR;
using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Enums;
using HealthTech.Application.Authentication.Queries.GetCurrentUser;
using HealthTech.Application.Authentication.DTOs;

namespace HealthTech.Application.Authentication.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, GetCurrentUserResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserService _userService;

    public GetCurrentUserQueryHandler(
        ICurrentUserService currentUserService,
        IUserService userService)
    {
        _currentUserService = currentUserService;
        _userService = userService;
    }

    public async Task<GetCurrentUserResponse> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return new GetCurrentUserResponse
                {
                    IsSuccess = false,
                    Message = "User not authenticated",
                    RequestId = request.RequestId
                };
            }

            var user = await _userService.GetUserByIdAsync(Guid.Parse(userId), _currentUserService.TenantId ?? "default");
            if (user == null)
            {
                return new GetCurrentUserResponse
                {
                    IsSuccess = false,
                    Message = "User not found",
                    RequestId = request.RequestId
                };
            }

            var scopes = await _userService.GetUserScopesAsync(user.Id);

            return new GetCurrentUserResponse
            {
                IsSuccess = true,
                Message = "User retrieved successfully",
                RequestId = request.RequestId,
                User = new UserInfoDto
                {
                    Id = user.Id.ToString(),
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.DisplayName,
                    Role = user.Role,
                    PractitionerId = user.PractitionerId,
                    TenantId = user.TenantId,
                    Scopes = scopes.ToList()
                }
            };
        }
        catch (Exception ex)
        {
            return new GetCurrentUserResponse
            {
                IsSuccess = false,
                Message = $"An error occurred while getting current user: {ex.Message}",
                RequestId = request.RequestId
            };
        }
    }
}
