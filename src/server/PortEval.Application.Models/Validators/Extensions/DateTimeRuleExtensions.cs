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
                .LessThanOrEqualTo(_ => DateTime.Now);
        }

        public static IRuleBuilder<T, DateTime?> ApplyTimeRangeRule<T>(this IRuleBuilder<T, DateTime?> builder)
        {
            return builder
                .GreaterThanOrEqualTo(PortEvalConstants.FinancialDataStartTime)
                .LessThanOrEqualTo(_ => DateTime.Now);
        }
    }
}
