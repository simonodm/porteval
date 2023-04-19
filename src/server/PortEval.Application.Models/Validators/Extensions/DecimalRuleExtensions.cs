using FluentValidation;

namespace PortEval.Application.Models.Validators.Extensions;

internal static class DecimalRuleExtensions
{
    private const int MinPrice = 0;
    private const int MaxPrice = 500_000;
    private const int MinAmount = -500_000;
    private const int MaxAmount = 500_000;

    public static IRuleBuilder<T, decimal> ApplyPriceRangeRule<T>(this IRuleBuilder<T, decimal> builder)
    {
        return builder
            .GreaterThan(MinPrice)
            .LessThanOrEqualTo(MaxPrice);
    }

    public static IRuleBuilder<T, decimal?> ApplyPriceRangeRule<T>(this IRuleBuilder<T, decimal?> builder)
    {
        return builder
            .GreaterThan(MinPrice)
            .LessThanOrEqualTo(MaxPrice);
    }

    public static IRuleBuilder<T, decimal> ApplyAmountRangeRule<T>(this IRuleBuilder<T, decimal> builder)
    {
        return builder
            .GreaterThan(MinAmount)
            .LessThanOrEqualTo(MaxAmount);
    }

    public static IRuleBuilder<T, decimal?> ApplyAmountRangeRule<T>(this IRuleBuilder<T, decimal?> builder)
    {
        return builder
            .GreaterThan(MinAmount)
            .LessThanOrEqualTo(MaxAmount);
    }
}