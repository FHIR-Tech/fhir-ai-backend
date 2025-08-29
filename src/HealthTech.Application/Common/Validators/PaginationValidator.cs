using FluentValidation;
using HealthTech.Application.Common.Base;

namespace HealthTech.Application.Common.Validators;

public class PaginationValidator : AbstractValidator<BasePagedRequest<object>>
{
    public PaginationValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 1000)
            .WithMessage("Page size must be between 1 and 1000");

        RuleFor(x => x.SortOrder)
            .Must(x => x?.ToLower() == "asc" || x?.ToLower() == "desc")
            .WithMessage("Sort order must be 'asc' or 'desc'");
    }
}
