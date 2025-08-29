using FluentValidation;
using HealthTech.Application.Common.Validators;

namespace HealthTech.Application.PatientAccess.Commands.RevokePatientAccess;

public class RevokePatientAccessCommandValidator : BaseValidator<RevokePatientAccessCommand>
{
    public RevokePatientAccessCommandValidator()
    {
        RuleFor(x => x.AccessId)
            .NotEmpty().WithMessage("Access ID is required");

        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters");
    }
}
