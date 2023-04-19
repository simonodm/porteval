using FluentValidation;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Models.Validators;

public class PortfolioDtoValidator : AbstractValidator<PortfolioDto>
{
    public PortfolioDtoValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(64);
        RuleFor(p => p.Note)
            .MaximumLength(255);
        RuleFor(p => p.CurrencyCode)
            .NotEmpty()
            .Length(3);
    }
}