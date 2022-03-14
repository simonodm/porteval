using FluentValidation;
using PortEval.Application.Models.DTOs;
using PortEval.Domain;

namespace PortEval.Application.Models.Validators
{
    public class InstrumentPriceDtoValidator : AbstractValidator<InstrumentPriceDto>
    {
        public InstrumentPriceDtoValidator()
        {
            RuleFor(p => p.Price)
                .GreaterThan(0);
            RuleFor(p => p.Time)
                .GreaterThanOrEqualTo(PortEvalConstants.FinancialDataStartTime);
        }
    }
}
