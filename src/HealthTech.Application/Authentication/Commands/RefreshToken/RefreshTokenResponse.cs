using HealthTech.Application.Common.Base;

namespace HealthTech.Application.Authentication.Commands.RefreshToken;

public record RefreshTokenResponse : BaseResponse
{
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
}
