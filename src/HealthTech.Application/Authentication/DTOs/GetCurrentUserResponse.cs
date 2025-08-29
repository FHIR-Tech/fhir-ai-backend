using HealthTech.Application.Authentication.DTOs;

namespace HealthTech.Application.Authentication.DTOs;

public record GetCurrentUserResponse
{
    public bool Success { get; init; }
    public UserInfoDto? User { get; init; }
    public string? ErrorMessage { get; init; }
}
