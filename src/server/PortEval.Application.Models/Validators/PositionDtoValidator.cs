using FluentValidation;
using PortEval.Application.Models.DTOs;
using PortEval.Domain;

namespace PortEval.Application.Models.Validators
{
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
                RuleFor(p => p.Amount).NotEmpty().GreaterThan(0);
                RuleFor(p => p.Price).NotEmpty().GreaterThan(0);
                RuleFor(p => p.Time).NotEmpty().GreaterThanOrEqualTo(PortEvalConstants.FinancialDataStartTime);
            });
        }
    }
}
