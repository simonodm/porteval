using System;
using System.Collections.Generic;
using System.Linq;
using PortEval.Application.Core.Interfaces.Calculators;

namespace PortEval.Application.Core.Common.Calculators;

/// <summary>
///     Calculates approximate internal rate of return of a position.
/// </summary>
public class PositionPerformanceCalculator : IPositionPerformanceCalculator
{
    /// <summary>
    ///     Calculates total approximate performance of provided positions in the given range using Internal Rate of Return.
    ///     IRR is approximated using Newton's method.
    /// </summary>
    /// <param name="positionPriceRangeData">Transaction and price data for each position.</param>
    /// <param name="from">Start range.</param>
    /// <param name="to">End range.</param>
    /// <returns>Total approximate performance of the provided positions.</returns>
    public decimal CalculatePerformance(IEnumerable<PositionPriceRangeData> positionPriceRangeData, DateTime from,
        DateTime to)
    {
        var firstTransactionTime = positionPriceRangeData
            .Select(p => p.Transactions.FirstOrDefault()?.Time)
            .Where(t => t != null)
            .Min();

        if (firstTransactionTime == null)
        {
            return 0;
        }

        var intervalCountBase = (DateTime)firstTransactionTime < from ? from : (DateTime)firstTransactionTime;
        var interval = GetSinglePointIntervalLength(intervalCountBase, to);
        var totalIntervalCount = CalculateIntervalPointCount(intervalCountBase, to, interval);
        var equation = new PolynomialEquation(0.001);

        // helper variables for initial guess calculation
        var totalSellValue = 0m;
        var totalPurchaseValue = 0m;
        var potentiallyUnrealizedValue = 0m;

        foreach (var positionPriceData in positionPriceRangeData)
        foreach (var transaction in positionPriceData.Transactions)
        {
            var transactionTime = transaction.Time < from ? from : transaction.Time;
            var transactionIntervalCount = CalculateIntervalPointCount(transactionTime, to, interval);
            var transactionPrice =
                transaction.Time < from ? positionPriceData.PriceAtRangeStart?.Price ?? 0m : transaction.Price;
            equation.AddCoefficient(transactionIntervalCount, -(double)(transaction.Amount * transactionPrice));
            equation.AddCoefficient(0, (double)(transaction.Amount * positionPriceData.PriceAtRangeEnd?.Price ?? 0m));
            if (transaction.Amount < 0)
            {
                totalSellValue += -transaction.Amount * transactionPrice;
                potentiallyUnrealizedValue += transaction.Amount * positionPriceData.PriceAtRangeEnd?.Price ?? 0m;
            }
            else
            {
                totalPurchaseValue += transaction.Amount * transactionPrice;
                potentiallyUnrealizedValue += transaction.Amount * positionPriceData.PriceAtRangeEnd?.Price ?? 0m;
            }
        }

        var initialGuess = 0m;

        try
        {
            initialGuess = (totalSellValue + potentiallyUnrealizedValue) / totalPurchaseValue;

            var singlePointPerformance =
                equation.CalculateRoot(Math.Pow((double)initialGuess, 1.0 / totalIntervalCount));
            var totalPerformance = Math.Pow(singlePointPerformance, totalIntervalCount);

            if (singlePointPerformance < 0 && totalPerformance > 0)
            {
                totalPerformance *= -1;
            }

            return (decimal)totalPerformance - 1;
        }
        catch (OverflowException)
        {
            return initialGuess - 1;
        }
    }

    /// <summary>
    ///     Calculates the amount of interval units between the supplied dates.
    /// </summary>
    /// <param name="rangeStart">Range start.</param>
    /// <param name="rangeEnd">Range end.</param>
    /// <param name="interval">Single interval unit duration.</param>
    /// <returns>Interval unit count between the supplied dates.</returns>
    private static int CalculateIntervalPointCount(DateTime rangeStart, DateTime rangeEnd, TimeSpan interval)
    {
        return (int)Math.Ceiling((rangeEnd.Ticks - (double)rangeStart.Ticks) / interval.Ticks);
    }

    /// <summary>
    ///     Calculates the appropriate interval unit length for the supplied date range.
    /// </summary>
    /// <param name="rangeStart">Range start.</param>
    /// <param name="rangeEnd">Range end.</param>
    /// <returns>Appropriate interval unit length.</returns>
    /// <remarks>
    ///     An appropriate interval unit length is such that the performance calculation will not run into double overflow
    ///     exceptions due to large exponents.
    /// </remarks>
    private static TimeSpan GetSinglePointIntervalLength(DateTime rangeStart, DateTime rangeEnd)
    {
        var difference = rangeEnd - rangeStart;
        if (difference >= TimeSpan.FromDays(3650))
        {
            return TimeSpan.FromDays(365);
        }

        if (difference >= TimeSpan.FromDays(365))
        {
            return TimeSpan.FromDays(30);
        }

        if (difference >= TimeSpan.FromDays(30))
        {
            return TimeSpan.FromDays(7);
        }

        if (difference >= TimeSpan.FromDays(2))
        {
            return TimeSpan.FromDays(1);
        }

        if (difference >= TimeSpan.FromHours(2))
        {
            return TimeSpan.FromHours(1);
        }

        return TimeSpan.FromMinutes(5);
    }
}