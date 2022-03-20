using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.Queries.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortEval.Application.Services.Queries.Helpers
{
    /// <summary>
    /// Calculates approximate internal rate of return of a position.
    /// </summary>
    internal static class InternalRateOfReturnCalculator
    {
        /// <summary>
        /// Calculates approximate internal rate of return in the given range using modified Newton's method.
        /// </summary>
        /// <param name="transactions">All transactions.</param>
        /// <param name="from">Start range.</param>
        /// <param name="to">End range.</param>
        /// <returns>An <see cref="EntityPerformanceDto">EntityPerformanceDto</see> instance containing the calculation result.</returns>
        public static EntityPerformanceDto CalculateIrr(IEnumerable<TransactionDetailsQueryModel> transactions, DateTime from, DateTime to)
        {
            var transactionsList = transactions.OrderBy(t => t.Time).ToList();
            if (transactionsList.Count == 0)
            {
                return new EntityPerformanceDto
                {
                    From = from,
                    To = to,
                    Performance = 0
                };
            }

            var firstTransactionTime = transactionsList[0].Time;
            var interval = GetSinglePointIntervalLength(firstTransactionTime, to);
            var totalIntervalCount = CalculateIntervalPointCount(firstTransactionTime, to, interval);
            var equation = new PolynomialEquation(0.01);
            var initialGuess = 0m;

            foreach (var transaction in transactionsList)
            {
                var transactionIntervalCount = CalculateIntervalPointCount(transaction.Time, to, interval);
                var transactionValue =
                    transaction.Time < from ? transaction.InstrumentPriceAtRangeStart : transaction.Price;
                equation.AddCoefficient(transactionIntervalCount, -(double)(transaction.Amount * transactionValue));
                equation.AddCoefficient(0, (double)(transaction.Amount * transaction.InstrumentPriceAtRangeEnd));
                initialGuess += transaction.Amount *
                    (transaction.InstrumentPriceAtRangeEnd - transaction.InstrumentPriceAtRangeStart)
                    / Math.Max(0, transaction.InstrumentPriceAtRangeStart) + 1;
            }

            var singlePointPerformance = equation.CalculateRoot(Math.Pow((double)initialGuess, 1.0 / totalIntervalCount));
            var totalPerformance = Math.Pow(singlePointPerformance, totalIntervalCount);

            if (singlePointPerformance < 0 && totalPerformance > 0) totalPerformance *= -1;

            return new EntityPerformanceDto
            {
                From = from,
                To = to,
                Performance = (decimal)totalPerformance - 1
            };
        }

        /// <summary>
        /// Calculates the amount of interval units between the supplied dates.
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
        /// Calculates the appropriate interval unit length for the supplied date range.
        /// </summary>
        /// <param name="rangeStart">Range start.</param>
        /// <param name="rangeEnd">Range end.</param>
        /// <returns>Appropriate interval unit length.</returns>
        /// <remarks>An appropriate interval unit length is such that the performance calculation will not run into double overflow exceptions due to large exponents.</remarks>
        private static TimeSpan GetSinglePointIntervalLength(DateTime rangeStart, DateTime rangeEnd)
        {
            var difference = rangeEnd - rangeStart;
            if (difference >= TimeSpan.FromDays(3650)) return TimeSpan.FromDays(365);
            if (difference >= TimeSpan.FromDays(365)) return TimeSpan.FromDays(30);
            if (difference >= TimeSpan.FromDays(30)) return TimeSpan.FromDays(7);
            if (difference >= TimeSpan.FromDays(2)) return TimeSpan.FromDays(1);
            if (difference >= TimeSpan.FromHours(2)) return TimeSpan.FromHours(1);

            return TimeSpan.FromMinutes(5);
        }
    }
}
