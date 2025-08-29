using HealthTech.Application.Common.Base;
using HealthTech.Domain.Enums;

namespace HealthTech.Application.PatientAccess.Commands.GrantPatientAccess;

public record GrantPatientAccessCommand : BaseRequest<GrantPatientAccessResponse>
{
    public string TargetUserId { get; init; } = string.Empty;
    public string PatientId { get; init; } = string.Empty;
    public PatientAccessLevel AccessLevel { get; init; }
    public string? Reason { get; init; }
    public DateTime? ExpiresAt { get; init; }
}
