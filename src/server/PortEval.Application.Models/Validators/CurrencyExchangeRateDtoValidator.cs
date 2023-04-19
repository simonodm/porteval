using FluentValidation;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators.Extensions;

namespace PortEval.Application.Models.Validators;

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
            .NotEmpty()
            .ApplyPriceRangeRule();
        RuleFor(r => r.Time)
            .NotEmpty()
            .ApplyTimeRangeRule();
    }
}