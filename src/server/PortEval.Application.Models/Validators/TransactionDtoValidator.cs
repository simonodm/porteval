using FluentValidation;
using PortEval.Application.Models.DTOs;
using PortEval.Domain;

namespace PortEval.Application.Models.Validators
{
    public class TransactionDtoValidator : AbstractValidator<TransactionDto>
    {
        public TransactionDtoValidator()
        {
            RuleFor(t => t.PositionId)
                .NotEmpty();
            RuleFor(t => t.Amount)
                .NotEmpty();
            RuleFor(t => t.Price)
                .NotEmpty()
                .GreaterThan(0);
            RuleFor(t => t.Time)
                .NotEmpty()
                .GreaterThanOrEqualTo(PortEvalConstants.FinancialDataStartTime);
            RuleFor(t => t.Note)
                .MaximumLength(255);
        }
    }
}
