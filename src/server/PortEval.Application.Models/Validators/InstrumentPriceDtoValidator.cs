using FluentValidation;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators.Extensions;

namespace PortEval.Application.Models.Validators
{
    public class InstrumentPriceDtoValidator : AbstractValidator<InstrumentPriceDto>
    {
        public InstrumentPriceDtoValidator()
        {
            RuleFor(p => p.InstrumentId)
                .NotEmpty();
            RuleFor(p => p.Price)
                .NotEmpty()
                .ApplyPriceRangeRule();
            RuleFor(p => p.Time)
                .NotEmpty()
                .ApplyTimeRangeRule();
        }
    }
}
