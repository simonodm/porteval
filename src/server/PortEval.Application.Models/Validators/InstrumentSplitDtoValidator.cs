using FluentValidation;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators.Extensions;

namespace PortEval.Application.Models.Validators
{
    public class InstrumentSplitDtoValidator : AbstractValidator<InstrumentSplitDto>
    {
        public InstrumentSplitDtoValidator()
        {
            RuleFor(s => s.InstrumentId)
                .NotEmpty();
            RuleFor(s => s.SplitRatioDenominator)
                .GreaterThan(0)
                .LessThanOrEqualTo(10000)
                .NotEmpty();
            RuleFor(s => s.SplitRatioNumerator)
                .GreaterThan(0)
                .LessThanOrEqualTo(10000)
                .NotEmpty();
            RuleFor(s => s.Time)
                .ApplyTimeRangeRule();
        }
    }
}
