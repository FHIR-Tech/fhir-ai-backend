using HealthTech.Application.Common.Base;

namespace HealthTech.Application.Authentication.Commands.Login;

public record LoginCommand : BaseRequest<LoginResponse>
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
