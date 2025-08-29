using HealthTech.Application.Authentication.DTOs;

namespace HealthTech.Application.Authentication.DTOs;

public record LoginResponse
{
    public bool Success { get; init; }
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public UserInfoDto? User { get; init; }
    public string? ErrorMessage { get; init; }
}
