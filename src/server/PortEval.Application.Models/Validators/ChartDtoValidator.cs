using FluentValidation;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Models.Validators
{
    public class ChartDtoValidator : AbstractValidator<ChartDto>
    {
        public ChartDtoValidator()
        {
            RuleFor(c => c.Name)
                .MinimumLength(3)
                .MaximumLength(32);
            RuleFor(c => c.DateRangeStart)
                .NotNull()
                .When(c => c.IsToDate != true);
            RuleFor(c => c.DateRangeEnd)
                .NotNull()
                .When(c => c.IsToDate != true);
            RuleFor(c => c.ToDateRange)
                .NotNull()
                .When(c => c.IsToDate == true);
            RuleFor(c => c.Frequency)
                .NotNull()
                .When(c => c.Type == ChartType.AggregatedProfit || c.Type == ChartType.AggregatedPerformance);
            When(c => c.Type == ChartType.Profit || c.Type == ChartType.Price ||
                           c.Type == ChartType.AggregatedProfit, () =>
                {
                    RuleFor(c => c.CurrencyCode).NotNull();
                    RuleFor(c => c.CurrencyCode).Length(3);
                });
        }
    }
}
