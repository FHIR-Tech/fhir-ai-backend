using HealthTech.Domain.Enums;

namespace HealthTech.Application.Authentication.DTOs;

public record UserInfo
{
    public string Id { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public UserRole Role { get; init; }
    public string? PractitionerId { get; init; }
    public string TenantId { get; init; } = string.Empty;
    public List<string> Scopes { get; init; } = new();
}
