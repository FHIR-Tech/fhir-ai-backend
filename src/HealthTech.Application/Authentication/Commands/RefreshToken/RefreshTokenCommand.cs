using MediatR;
using HealthTech.Application.Authentication.DTOs;

namespace HealthTech.Application.Authentication.Commands.RefreshToken;

public record RefreshTokenCommand : IRequest<RefreshTokenResponse>
{
    public string RefreshToken { get; init; } = string.Empty;
}
