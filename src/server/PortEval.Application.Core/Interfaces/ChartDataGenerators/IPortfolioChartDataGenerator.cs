using System.Collections.Generic;
using PortEval.Application.Core.Common;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.Interfaces.ChartDataGenerators;

/// <summary>
///     Generates portfolio chart lines data.
/// </summary>
public interface IPortfolioChartDataGenerator
{
    /// <summary>
    ///     Generates data for portfolio price charts.
    /// </summary>
    /// <param name="portfolioPositionsPriceListData">
    ///     A collection of positions' price list data, see <see cref="PortfolioPositionsPriceListData" />.
    /// </param>
    /// <param name="dateRange">Date range of the chart.</param>
    /// <param name="frequency">Chart point interval.</param>
    /// <returns>An <see cref="IEnumerable{EntityChartPointDto}" /> containing chart line points.</returns>
    public IEnumerable<EntityChartPointDto> ChartValue(PortfolioPositionsPriceListData portfolioPositionsPriceListData,
        DateRangeParams dateRange, AggregationFrequency frequency);

    /// <summary>
    ///     Generates data for portfolio profit charts.
    /// </summary>
    /// <param name="portfolioPositionsPriceListData">
    ///     A collection of positions' price list data, see <see cref="PortfolioPositionsPriceListData" />.
    /// </param>
    /// <param name="dateRange">Date range of the chart.</param>
    /// <param name="frequency">Chart point interval.</param>
    /// <returns>An <see cref="IEnumerable{EntityChartPointDto}" /> containing chart line points.</returns>
    public IEnumerable<EntityChartPointDto> ChartProfit(PortfolioPositionsPriceListData portfolioPositionsPriceListData,
        DateRangeParams dateRange, AggregationFrequency frequency);

    /// <summary>
    ///     Generates data for portfolio performance charts.
    /// </summary>
    /// <param name="portfolioPositionsPriceListData">
    ///     A collection of positions' price list data, see <see cref="PortfolioPositionsPriceListData" />.
    /// </param>
    /// <param name="dateRange">Date range of the chart.</param>
    /// <param name="frequency">Chart point interval.</param>
    /// <returns>An <see cref="IEnumerable{EntityChartPointDto}" /> containing chart line points.</returns>
    public IEnumerable<EntityChartPointDto> ChartPerformance(
        PortfolioPositionsPriceListData portfolioPositionsPriceListData,
        DateRangeParams dateRange, AggregationFrequency frequency);

    /// <summary>
    ///     Generates data for portfolio aggregated profit charts.
    /// </summary>
    /// <param name="portfolioPositionsPriceListData">
    ///     A collection of positions' price list data, see <see cref="PortfolioPositionsPriceListData" />.
    /// </param>
    /// <param name="dateRange">Date range of the chart.</param>
    /// <param name="frequency">Chart point interval.</param>
    /// <returns>An <see cref="IEnumerable{EntityChartPointDto}" /> containing chart line points.</returns>
    public IEnumerable<EntityChartPointDto> ChartAggregatedProfit(
        PortfolioPositionsPriceListData portfolioPositionsPriceListData,
        DateRangeParams dateRange, AggregationFrequency frequency);

    /// <summary>
    ///     Generates data for portfolio aggregated performance charts.
    /// </summary>
    /// <param name="portfolioPositionsPriceListData">
    ///     A collection of positions' price list data, see <see cref="PortfolioPositionsPriceListData" />.
    /// </param>
    /// <param name="dateRange">Date range of the chart.</param>
    /// <param name="frequency">Chart point interval.</param>
    /// <returns>An <see cref="IEnumerable{EntityChartPointDto}" /> containing chart line points.</returns>
    public IEnumerable<EntityChartPointDto> ChartAggregatedPerformance(
        PortfolioPositionsPriceListData portfolioPositionsPriceListData,
        DateRangeParams dateRange, AggregationFrequency frequency);
}