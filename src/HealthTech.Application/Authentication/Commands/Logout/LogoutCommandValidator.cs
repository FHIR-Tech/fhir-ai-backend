using FluentValidation;
using HealthTech.Application.Common.Validators;

namespace HealthTech.Application.Authentication.Commands.Logout;

public class LogoutCommandValidator : BaseValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.SessionToken)
            .NotEmpty().WithMessage("Session token is required");
    }
}
