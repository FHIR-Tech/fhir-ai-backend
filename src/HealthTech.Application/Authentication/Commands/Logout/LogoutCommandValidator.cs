using FluentValidation;
using HealthTech.Application.Authentication.Commands.Logout;

namespace HealthTech.Application.Authentication.Commands.Logout;

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.SessionToken)
            .NotEmpty().WithMessage("Session token is required");
    }
}
