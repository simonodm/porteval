using PortEval.Application.Services.Queries.Calculators.Interfaces;
using PortEval.Application.Services.Queries.Helpers;
using PortEval.Application.Services.Queries.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortEval.Application.Services.Queries.Calculators
{
    /// <summary>
    /// Calculates approximate internal rate of return of a position.
    /// </summary>
    internal class IrrPerformanceCalculator : ITransactionBasedPerformanceCalculator
    {
        /// <summary>
        /// Calculates approximate performance in the given range using Internal Rate of Return.
        ///
        /// IRR is approximated using Newton's method.
        /// </summary>
        /// <param name="transactions">All transactions.</param>
        /// <param name="from">Start range.</param>
        /// <param name="to">End range.</param>
        /// <returns>Total performance of the financial entity represented by the provided transactions.</returns>
        public decimal CalculatePerformance(IEnumerable<TransactionDetailsQueryModel> transactions, DateTime from, DateTime to)
        {
            var transactionsList = transactions.OrderBy(t => t.Time).ToList();
            if (transactionsList.Count == 0)
            {
                return 0;
            }

            var firstTransactionTime = transactionsList[0].Time;
            var intervalCountBase = firstTransactionTime < from ? from : firstTransactionTime;
            var interval = GetSinglePointIntervalLength(intervalCountBase, to);
            var totalIntervalCount = CalculateIntervalPointCount(intervalCountBase, to, interval);
            var equation = new PolynomialEquation(0.001);

            // helper variables for initial guess calculation
            var totalSellValue = 0m;
            var totalPurchaseValue = 0m;
            var potentiallyUnrealizedValue = 0m;

            foreach (var transaction in transactionsList)
            {
                var transactionTime = transaction.Time < from ? from : transaction.Time;
                var transactionIntervalCount = CalculateIntervalPointCount(transactionTime, to, interval);
                var transactionPrice =
                    transaction.Time < from ? transaction.InstrumentPriceAtRangeStart : transaction.Price;
                equation.AddCoefficient(transactionIntervalCount, -(double)(transaction.Amount * transactionPrice));
                equation.AddCoefficient(0, (double)(transaction.Amount * transaction.InstrumentPriceAtRangeEnd));
                if (transaction.Amount < 0)
                {
                    totalSellValue += -transaction.Amount * transactionPrice;
                    potentiallyUnrealizedValue += transaction.Amount * transaction.InstrumentPriceAtRangeEnd;
                }
                else
                {
                    totalPurchaseValue += transaction.Amount * transactionPrice;
                    potentiallyUnrealizedValue += transaction.Amount * transaction.InstrumentPriceAtRangeEnd;
                }
            }

            var initialGuess = 0m;

            try
            {
                initialGuess = (totalSellValue + potentiallyUnrealizedValue) / totalPurchaseValue;

                var singlePointPerformance = equation.CalculateRoot(Math.Pow((double)initialGuess, 1.0 / totalIntervalCount));
                var totalPerformance = Math.Pow(singlePointPerformance, totalIntervalCount);

                if (singlePointPerformance < 0 && totalPerformance > 0) totalPerformance *= -1;

                return (decimal)totalPerformance - 1;
            }
            catch (OverflowException)
            {
                var simplifiedPerformance = Math.Pow((double)initialGuess, totalIntervalCount);
                return (decimal)simplifiedPerformance - 1;
            }
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
