using MediatR;
using HealthTech.Application.Authentication.DTOs;

namespace HealthTech.Application.Authentication.Commands.Login;

public record LoginCommand : IRequest<LoginResponse>
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? TenantId { get; init; }
}
