using PortEval.Application.Features.Common;
using System;
using System.Collections.Generic;

namespace PortEval.Application.Features.Interfaces.Calculators
{
    public interface IPositionPerformanceCalculator
    {
        /// <summary>
        /// Calculates the total performance of provided positions based on their transactions and instruments' prices.
        /// </summary>
        /// <param name="positionPriceRangeData">Transaction and price data for each position.</param>
        /// <param name="from">Date range start.</param>
        /// <param name="to">Date range end.</param>
        /// <returns>The total performance of the provided positions between <paramref name="from"/> and <paramref name="to"/>.</returns>
        public decimal CalculatePerformance(IEnumerable<PositionPriceRangeData> positionPriceRangeData, DateTime from, DateTime to);
    }
}
