using HealthTech.Application.Common.Base;

namespace HealthTech.Application.Authentication.Commands.RefreshToken;

public record RefreshTokenCommand : BaseRequest<RefreshTokenResponse>
{
    public string RefreshToken { get; init; } = string.Empty;
}
