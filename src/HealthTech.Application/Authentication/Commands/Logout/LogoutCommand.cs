using HealthTech.Application.Common.Base;

namespace HealthTech.Application.Authentication.Commands.Logout;

public record LogoutCommand : BaseRequest<LogoutResponse>
{
    public string SessionToken { get; init; } = string.Empty;
}
