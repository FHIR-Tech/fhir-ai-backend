using MediatR;
using HealthTech.Application.Authentication.DTOs;

namespace HealthTech.Application.Authentication.Commands.Logout;

public record LogoutCommand : IRequest<LogoutResponse>
{
    public string SessionToken { get; init; } = string.Empty;
}
