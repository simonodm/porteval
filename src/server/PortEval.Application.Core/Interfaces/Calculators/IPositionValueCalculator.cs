using PortEval.Application.Core.Common;
using System;
using System.Collections.Generic;

namespace PortEval.Application.Core.Interfaces.Calculators
{
    /// <summary>
    /// Calculates a position's value.
    /// </summary>
    public interface IPositionValueCalculator
    {
        /// <summary>
        /// Calculates the total value of provided positions based on their transactions and instruments' prices.
        /// </summary>
        /// <param name="positionsPriceData">Price and transaction data for each position.</param>
        /// <param name="time">Time at which the value should be calculated.</param>
        /// <returns>The value of the entity at <paramref name="time" />.</returns>
        public decimal CalculateValue(IEnumerable<PositionPriceRangeData> positionsPriceData, DateTime time);
    }
}
