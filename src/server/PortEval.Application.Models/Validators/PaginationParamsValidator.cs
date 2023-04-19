using FluentValidation;
using PortEval.Application.Models.QueryParams;

namespace PortEval.Application.Models.Validators;

public class PaginationParamsValidator : AbstractValidator<PaginationParams>
{
    public PaginationParamsValidator()
    {
        RuleFor(p => p.Limit)
            .GreaterThan(0)
            .LessThanOrEqualTo(300);
        RuleFor(p => p.Page)
            .GreaterThan(0);
    }
}