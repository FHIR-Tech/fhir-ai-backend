using HealthTech.Application.Common.Base;
using HealthTech.Domain.Enums;

namespace HealthTech.Application.PatientAccess.Queries.GetPatientAccess;

public record GetPatientAccessResponse : PagedResponse<PatientAccessInfo>
{
    // Additional properties specific to patient access list if needed
}

public record PatientAccessInfo
{
    public string Id { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string PatientId { get; init; } = string.Empty;
    public string PatientName { get; init; } = string.Empty;
    public PatientAccessLevel AccessLevel { get; init; }
    public string? Reason { get; init; }
    public DateTime GrantedAt { get; init; }
    public string GrantedBy { get; init; } = string.Empty;
    public DateTime? ExpiresAt { get; init; }
    public bool IsActive { get; init; }
}
