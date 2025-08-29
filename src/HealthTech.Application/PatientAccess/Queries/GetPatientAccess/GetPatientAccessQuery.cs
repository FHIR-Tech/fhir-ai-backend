using HealthTech.Application.Common.Base;
using HealthTech.Domain.Enums;

namespace HealthTech.Application.PatientAccess.Queries.GetPatientAccess;

public record GetPatientAccessQuery : BasePagedRequest<GetPatientAccessResponse>
{
    public string? PatientId { get; init; }
    public string? UserId { get; init; }
    public PatientAccessLevel? AccessLevel { get; init; }
    public bool? IsActive { get; init; }
}
