using FluentValidation;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Models.Validators
{
    public class PositionDtoValidator : AbstractValidator<PositionDto>
    {
        public PositionDtoValidator()
        {
            RuleFor(p => p.PortfolioId)
                .NotNull();
            RuleFor(p => p.InstrumentId)
                .NotNull();
            RuleFor(p => p.Note)
                .MaximumLength(255);
        }
    }
}
