using System;
using System.Collections.Generic;
using PortEval.Application.Core.Common;

namespace PortEval.Application.Core.Interfaces.Calculators
{
    /// <summary>
    /// Calculates a position's profit.
    /// </summary>
    public interface IPositionProfitCalculator
    {
        /// <summary>
        /// Calculates the total profit of provided positions based on their transactions and instruments' prices.
        /// </summary>
        /// <param name="positionPriceRangeData">Price and transaction data for each position.</param>
        /// <param name="from">Date range start.</param>
        /// <param name="to">Date range end.</param>
        /// <returns>The total profit of the provided positions between <paramref name="from"/> and <paramref name="to"/>.</returns>
        public decimal CalculateProfit(IEnumerable<PositionPriceRangeData> positionPriceRangeData, DateTime from, DateTime to);
    }
}
