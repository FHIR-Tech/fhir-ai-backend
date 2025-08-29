using MediatR;
using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Enums;

namespace HealthTech.Application.Authentication.Queries;

public record GetCurrentUserQuery : IRequest<GetCurrentUserResponse>;

public record GetCurrentUserResponse
{
    public bool Success { get; init; }
    public UserInfo? User { get; init; }
    public string? ErrorMessage { get; init; }
}

public record UserInfo
{
    public string Id { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public UserRole Role { get; init; }
    public string? PractitionerId { get; init; }
    public string TenantId { get; init; } = string.Empty;
    public List<string> Scopes { get; init; } = new();
}

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
                    Success = false,
                    ErrorMessage = "User not authenticated"
                };
            }

            var user = await _userService.GetUserByIdAsync(Guid.Parse(userId), _currentUserService.TenantId ?? "default");
            if (user == null)
            {
                return new GetCurrentUserResponse
                {
                    Success = false,
                    ErrorMessage = "User not found"
                };
            }

            var scopes = await _userService.GetUserScopesAsync(user.Id);

            return new GetCurrentUserResponse
            {
                Success = true,
                User = new UserInfo
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
                Success = false,
                ErrorMessage = $"An error occurred while getting current user: {ex.Message}"
            };
        }
    }
}
