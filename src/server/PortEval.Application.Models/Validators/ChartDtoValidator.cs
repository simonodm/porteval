using FluentValidation;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators.Extensions;
using PortEval.Domain;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Models.Validators;

public class ChartDtoValidator : AbstractValidator<ChartDto>
{
    public ChartDtoValidator()
    {
        RuleFor(c => c.Name)
            .MinimumLength(3)
            .MaximumLength(64);
        RuleFor(c => c.DateRangeStart)
            .NotEmpty()
            .GreaterThanOrEqualTo(PortEvalConstants.FinancialDataStartTime)
            .When(c => c.IsToDate != true);
        RuleFor(c => c.DateRangeEnd)
            .NotEmpty()
            .ApplyTimeRangeRule()
            .GreaterThanOrEqualTo(c => c.DateRangeStart)
            .When(c => c.IsToDate != true);
        RuleFor(c => c.ToDateRange)
            .NotEmpty()
            .When(c => c.IsToDate == true);
        RuleFor(c => c.Frequency)
            .NotEmpty()
            .When(c => c.Type is ChartType.AggregatedProfit or ChartType.AggregatedPerformance);
        When(c => c.Type is ChartType.Profit or ChartType.Price or ChartType.AggregatedProfit, () =>
        {
            RuleFor(c => c.CurrencyCode).NotEmpty();
            RuleFor(c => c.CurrencyCode).Length(3);
        });

        RuleForEach(c => c.Lines)
            .SetValidator(new ChartLineDtoValidator());
    }
}