using PortEval.Application.Features.Common;
using System;
using System.Collections.Generic;

namespace PortEval.Application.Features.Interfaces.Calculators
{
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
