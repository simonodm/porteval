using FluentValidation;
using PortEval.Domain;
using System;

namespace PortEval.Application.Models.Validators.Extensions
{
    internal static class DateTimeRuleExtensions
    {
        public static IRuleBuilder<T, DateTime> ApplyTimeRangeRule<T>(this IRuleBuilder<T, DateTime> builder)
        {
            return builder
                .GreaterThanOrEqualTo(PortEvalConstants.FinancialDataStartTime)
                .LessThanOrEqualTo(_ => DateTime.UtcNow);
        }

        public static IRuleBuilder<T, DateTime?> ApplyTimeRangeRule<T>(this IRuleBuilder<T, DateTime?> builder)
        {
            return builder
                .GreaterThanOrEqualTo(PortEvalConstants.FinancialDataStartTime)
                .LessThanOrEqualTo(_ => DateTime.UtcNow);
        }
    }
}
