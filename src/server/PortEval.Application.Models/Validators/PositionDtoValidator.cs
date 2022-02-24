using FluentValidation;
using PortEval.Application.Models.DTOs;

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

            When(p => p.InitialTransaction != default, () =>
            {
                RuleFor(p => p.InitialTransaction.Amount).NotEmpty().GreaterThan(0);
                RuleFor(p => p.InitialTransaction.Price).NotEmpty().GreaterThan(0);
                RuleFor(p => p.InitialTransaction.Time).NotEmpty();
            });
        }
    }
}
