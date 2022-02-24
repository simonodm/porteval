using FluentValidation;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Models.Validators
{
    public class InstrumentDtoValidator : AbstractValidator<InstrumentDto>
    {
        public InstrumentDtoValidator()
        {
            RuleFor(i => i.Symbol)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(10);
            RuleFor(i => i.Name)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(64);
            RuleFor(i => i.Exchange)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(32);
            RuleFor(i => i.CurrencyCode)
                .NotEmpty()
                .Length(3);
            RuleFor(i => i.Note)
                .MaximumLength(255);
        }
    }
}
