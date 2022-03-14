using FluentValidation;
using PortEval.Application.Models.DTOs;
using PortEval.Domain;

namespace PortEval.Application.Models.Validators
{
    public class CurrencyExchangeRateDtoValidator : AbstractValidator<CurrencyExchangeRateDto>
    {
        public CurrencyExchangeRateDtoValidator()
        {
            RuleFor(r => r.CurrencyFromCode)
                .NotEmpty()
                .Length(3);
            RuleFor(r => r.CurrencyToCode)
                .NotEmpty()
                .Length(3);
            RuleFor(r => r.ExchangeRate)
                .GreaterThan(0);
            RuleFor(r => r.Time)
                .GreaterThanOrEqualTo(PortEvalConstants.FinancialDataStartTime);
        }
    }
}
