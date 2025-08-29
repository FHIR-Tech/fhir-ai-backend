using HealthTech.Application.Common.Base;

namespace HealthTech.Application.PatientAccess.Commands.RevokePatientAccess;

public record RevokePatientAccessCommand : BaseRequest<RevokePatientAccessResponse>
{
    public string AccessId { get; init; } = string.Empty;
    public string? Reason { get; init; }
}
