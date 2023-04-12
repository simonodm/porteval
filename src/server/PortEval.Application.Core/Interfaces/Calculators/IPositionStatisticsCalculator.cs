using PortEval.Application.Core.Common;
using PortEval.Application.Models.DTOs;
using System;

namespace PortEval.Application.Core.Interfaces.Calculators
{
    /// <summary>
    /// Calculates the statistics of a position.
    /// </summary>
    public interface IPositionStatisticsCalculator
    {
        /// <summary>
        /// Calculates the statistics of a position at the specified time based on its transactions and the underlying instrument's prices.
        /// </summary>
        /// <param name="positionPriceData">Position's transactions and its instrument's prices.</param>
        /// <param name="time">Time to calculate statistics at.</param>
        /// <returns>A <see cref="PositionStatisticsDto"/> instance containing calculated statistics.</returns>
        PositionStatisticsDto CalculateStatistics(
            PositionPriceListData positionPriceData, DateTime time);
    }
}