using FluentValidation;
using HealthTech.Application.Common.Validators;
using HealthTech.Domain.Enums;

namespace HealthTech.Application.PatientAccess.Queries.GetPatientAccess;

public class GetPatientAccessQueryValidator : BaseValidator<GetPatientAccessQuery>
{
    public GetPatientAccessQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.AccessLevel)
            .IsInEnum().When(x => x.AccessLevel.HasValue)
            .WithMessage("Invalid access level");
    }
}
