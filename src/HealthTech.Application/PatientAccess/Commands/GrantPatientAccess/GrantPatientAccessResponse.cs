using HealthTech.Application.Common.Base;

namespace HealthTech.Application.PatientAccess.Commands.GrantPatientAccess;

public record GrantPatientAccessResponse : BaseResponse
{
    public string? AccessId { get; init; }
}
