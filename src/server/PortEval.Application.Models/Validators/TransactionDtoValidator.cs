using FluentValidation;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Models.Validators
{
    public class TransactionDtoValidator : AbstractValidator<TransactionDto>
    {
        public TransactionDtoValidator()
        {
            RuleFor(t => t.PositionId)
                .NotNull();
            RuleFor(t => t.PortfolioId)
                .NotNull();
            RuleFor(t => t.Amount)
                .NotNull();
            RuleFor(t => t.Price)
                .NotNull()
                .GreaterThan(0);
            RuleFor(t => t.Time)
                .NotNull();
            RuleFor(t => t.Note)
                .MaximumLength(255);
        }
    }
}
