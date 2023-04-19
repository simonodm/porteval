using FluentValidation;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators.Extensions;

namespace PortEval.Application.Models.Validators;

public class PositionDtoValidator : AbstractValidator<PositionDto>
{
    public PositionDtoValidator()
    {
        RuleFor(p => p.PortfolioId)
            .NotEmpty();
        RuleFor(p => p.InstrumentId)
            .NotEmpty();
        RuleFor(p => p.Note)
            .MaximumLength(255);

        When(p => p.Id == default, () =>
        {
            RuleFor(p => p.Amount)
                .NotEmpty()
                .ApplyAmountRangeRule()
                .GreaterThan(0);
            RuleFor(p => p.Price)
                .NotEmpty()
                .ApplyPriceRangeRule();
            RuleFor(p => p.Time)
                .NotEmpty()
                .ApplyTimeRangeRule();
        });
    }
}