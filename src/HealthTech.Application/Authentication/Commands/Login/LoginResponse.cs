using HealthTech.Application.Authentication.DTOs;
using HealthTech.Application.Common.Base;

namespace HealthTech.Application.Authentication.Commands.Login;

public record LoginResponse : BaseResponse
{
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public UserInfoDto? User { get; init; }
}
