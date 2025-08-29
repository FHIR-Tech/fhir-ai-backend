using FluentValidation;
using HealthTech.Application.Common.Validators;

namespace HealthTech.Application.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandValidator : BaseValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required");
    }
}
