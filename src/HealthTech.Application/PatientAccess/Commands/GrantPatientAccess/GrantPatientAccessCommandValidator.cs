using FluentValidation;
using HealthTech.Application.Common.Validators;
using HealthTech.Domain.Enums;

namespace HealthTech.Application.PatientAccess.Commands.GrantPatientAccess;

public class GrantPatientAccessCommandValidator : BaseValidator<GrantPatientAccessCommand>
{
    public GrantPatientAccessCommandValidator()
    {
        RuleFor(x => x.TargetUserId)
            .NotEmpty().WithMessage("Target user ID is required");

        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient ID is required");

        RuleFor(x => x.AccessLevel)
            .IsInEnum().WithMessage("Invalid access level");

        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters");

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTime.UtcNow).When(x => x.ExpiresAt.HasValue)
            .WithMessage("Expiration date must be in the future");
    }
}
