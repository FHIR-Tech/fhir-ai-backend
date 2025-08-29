namespace HealthTech.Application.Authentication.DTOs;

public record RefreshTokenResponse
{
    public bool Success { get; init; }
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public string? ErrorMessage { get; init; }
}
