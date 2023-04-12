using PortEval.Application.Core.Common;
using PortEval.Application.Models.DTOs;
using System;

namespace PortEval.Application.Core.Interfaces.Calculators
{
    /// <summary>
    /// Calculates the statistics of a portfolio.
    /// </summary>
    public interface IPortfolioStatisticsCalculator
    {
        /// <summary>
        /// Calculates the statistics of a portfolio at the specified time based on its positions' transactions and underlying instruments' prices.
        /// </summary>
        /// <param name="data">Positions' transactions and their instruments' prices.</param>
        /// <param name="time">Time to calculate statistics at.</param>
        /// <returns>An <see cref="EntityStatisticsDto"/> instance containing calculated statistics.</returns>
        EntityStatisticsDto CalculateStatistics(
            PortfolioPositionsPriceListData data, DateTime time);
    }
}