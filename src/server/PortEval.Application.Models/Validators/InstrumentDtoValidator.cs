using FluentValidation;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Models.Validators
{
    public class InstrumentDtoValidator : AbstractValidator<InstrumentDto>
    {
        public InstrumentDtoValidator()
        {
            RuleFor(i => i.Symbol)
                .NotNull()
                .MinimumLength(1)
                .MaximumLength(10);
            RuleFor(i => i.Name)
                .NotNull()
                .MinimumLength(1)
                .MaximumLength(32);
            RuleFor(i => i.Exchange)
                .NotNull()
                .MinimumLength(1)
                .MaximumLength(32);
            RuleFor(i => i.CurrencyCode)
                .NotNull()
                .Length(3);
        }
    }
}
