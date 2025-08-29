namespace HealthTech.Application.Authentication.DTOs;

public record LogoutResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}
