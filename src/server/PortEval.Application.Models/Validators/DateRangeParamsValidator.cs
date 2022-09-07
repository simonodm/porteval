using FluentValidation;
using PortEval.Application.Models.QueryParams;
using PortEval.Application.Models.Validators.Extensions;

namespace PortEval.Application.Models.Validators
{
    public class DateRangeParamsValidator : AbstractValidator<DateRangeParams>
    {
        public DateRangeParamsValidator()
        {
            RuleFor(dr => dr.From)
                .LessThan(dr => dr.To);
            RuleFor(dr => dr.To)
                .ApplyTimeRangeRule();
        }
    }
}
