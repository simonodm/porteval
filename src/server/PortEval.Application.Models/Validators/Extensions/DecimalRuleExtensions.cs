using FluentValidation;

namespace PortEval.Application.Models.Validators.Extensions
{
    internal static class DecimalRuleExtensions
    {
        private const int MIN_PRICE = 0;
        private const int MAX_PRICE = 500_000;
        private const int MIN_AMOUNT = -500_000;
        private const int MAX_AMOUNT = 500_000;

        public static IRuleBuilder<T, decimal> ApplyPriceRangeRule<T>(this IRuleBuilder<T, decimal> builder)
        {
            return builder
                .GreaterThan(MIN_PRICE)
                .LessThanOrEqualTo(MAX_PRICE);
        }

        public static IRuleBuilder<T, decimal?> ApplyPriceRangeRule<T>(this IRuleBuilder<T, decimal?> builder)
        {
            return builder
                .GreaterThan(MIN_PRICE)
                .LessThanOrEqualTo(MAX_PRICE);
        }

        public static IRuleBuilder<T, decimal> ApplyAmountRangeRule<T>(this IRuleBuilder<T, decimal> builder)
        {
            return builder
                .GreaterThan(MIN_AMOUNT)
                .LessThanOrEqualTo(MAX_AMOUNT);
        }

        public static IRuleBuilder<T, decimal?> ApplyAmountRangeRule<T>(this IRuleBuilder<T, decimal?> builder)
        {
            return builder
                .GreaterThan(MIN_AMOUNT)
                .LessThanOrEqualTo(MAX_AMOUNT);
        }
    }
}
