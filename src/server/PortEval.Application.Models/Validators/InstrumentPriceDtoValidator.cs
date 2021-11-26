using FluentValidation;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Models.Validators
{
    public class InstrumentPriceDtoValidator : AbstractValidator<InstrumentPriceDto>
    {
        public InstrumentPriceDtoValidator()
        {
            RuleFor(p => p.Price)
                .GreaterThan(0);
        }
    }
}
