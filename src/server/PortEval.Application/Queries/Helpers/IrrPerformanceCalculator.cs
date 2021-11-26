using PortEval.Application.Models.DTOs;
using PortEval.Application.Queries.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortEval.Application.Queries.Helpers
{
    /// <summary>
    /// Calculates approximate financial performance based on transactions' internal rate of return.
    /// </summary>
    internal static class IrrPerformanceCalculator
    {
        /// <summary>
        /// Calculates approximate performance in the given range using Newton's method.
        /// </summary>
        /// <param name="transactions">All transactions.</param>
        /// <param name="from">Start range.</param>
        /// <param name="to">End range.</param>
        /// <returns>An <see cref="EntityPerformanceDto">EntityPerformanceDto</see> instance containing the calculation result.</returns>
        public static EntityPerformanceDto CalculatePerformance(IEnumerable<TransactionDetailsQueryModel> transactions, DateTime from, DateTime to)
        {
            var transactionsList = transactions.ToList();
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

            foreach (var transaction in transactionsList)
            {
                var transactionIntervalCount = CalculateIntervalPointCount(transaction.Time, to, interval);
                var transactionValue =
                    transaction.Time < from ? transaction.InstrumentPriceAtRangeStart : transaction.Price;
                equation.AddCoefficient(transactionIntervalCount, (double)(transaction.Amount * transactionValue) / 1000);
                equation.AddCoefficient(0, -(double)(transaction.Amount * transaction.InstrumentPriceAtRangeEnd) / 1000);
            }

            var singlePointPerformance = equation.CalculateRoot(1);
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
            return (int)((rangeEnd.Ticks - rangeStart.Ticks) / interval.Ticks);
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
            if (difference >= TimeSpan.FromDays(1)) return TimeSpan.FromDays(1);

            return TimeSpan.FromHours(1);
        }
    }
}
