using FluentValidation;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Models.Validators
{
    public class CurrencyExchangeRateDtoValidator : AbstractValidator<CurrencyExchangeRateDto>
    {
        public CurrencyExchangeRateDtoValidator()
        {
            RuleFor(r => r.CurrencyFromCode)
                .NotNull()
                .Length(3);
            RuleFor(r => r.CurrencyToCode)
                .NotNull()
                .Length(3);
            RuleFor(r => r.ExchangeRate)
                .GreaterThan(0);
        }
    }
}
