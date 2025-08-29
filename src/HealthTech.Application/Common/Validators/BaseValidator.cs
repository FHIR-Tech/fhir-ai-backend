using FluentValidation;
using MediatR;

namespace HealthTech.Application.Common.Validators;

public abstract class BaseValidator<T> : AbstractValidator<T> where T : IRequest<object>
{
    protected BaseValidator()
    {
        // Common validation rules can be added here if needed
    }
}
