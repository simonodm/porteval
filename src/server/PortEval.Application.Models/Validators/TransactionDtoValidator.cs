using FluentValidation;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators.Extensions;

namespace PortEval.Application.Models.Validators
{
    public class TransactionDtoValidator : AbstractValidator<TransactionDto>
    {
        public TransactionDtoValidator()
        {
            RuleFor(t => t.PositionId)
                .NotEmpty();
            RuleFor(t => t.Amount)
                .NotEmpty()
                .ApplyAmountRangeRule();
            RuleFor(t => t.Price)
                .NotEmpty()
                .ApplyPriceRangeRule();
            RuleFor(t => t.Time)
                .NotEmpty()
                .ApplyTimeRangeRule();
            RuleFor(t => t.Note)
                .MaximumLength(255);
        }
    }
}
