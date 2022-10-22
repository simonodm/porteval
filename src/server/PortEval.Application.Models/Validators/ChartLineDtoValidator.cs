using FluentValidation;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.DTOs.Enums;

namespace PortEval.Application.Models.Validators
{
    public class ChartLineDtoValidator : AbstractValidator<ChartLineDto>
    {
        public ChartLineDtoValidator()
        {
            RuleFor(line => line.Width)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(8);
            RuleFor(line => line.InstrumentId)
                .NotEmpty()
                .When(line => line.Type == ChartLineType.Instrument);
            RuleFor(line => line.PortfolioId)
                .NotEmpty()
                .When(line => line.Type == ChartLineType.Portfolio);
            RuleFor(line => line.PositionId)
                .NotEmpty()
                .When(line => line.Type == ChartLineType.Position);

        }
    }
}
