using PortEval.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using PortEval.Application.Features.Common;

namespace PortEval.Application.Features.Interfaces.Calculators
{
    public interface IPositionValueCalculator
    {
        /// <summary>
        /// Calculates the total value of provided positions based on their transactions and instruments' prices.
        /// </summary>
        /// <param name="positionsPriceData">Price and transaction data for each position.</param>
        /// <param name="time">Time at which the value should be calculated.</param>
        /// <returns>The value of the entity at <paramref name="time" />.</returns>
        public decimal CalculateValue(IEnumerable<PositionPriceData> positionsPriceData, DateTime time);
    }
}
