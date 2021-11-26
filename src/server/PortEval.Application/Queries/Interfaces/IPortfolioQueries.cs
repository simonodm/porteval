using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Queries.Interfaces
{
    /// <summary>
    /// High performance read-only portfolio queries.
    /// </summary>
    public interface IPortfolioQueries
    {
        /// <summary>
        /// Retrieves all portfolios ordered by ID.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains an <c>IEnumerable</c> containing the retrieved portfolio DTOs.
        /// </returns>
        Task<QueryResponse<IEnumerable<PortfolioDto>>> GetPortfolios();

        /// <summary>
        /// Retrieves a portfolio by ID.
        /// </summary>
        /// <param name="portfolioId">ID of the portfolio to retrieve.</param>
        /// <returns>A task representing the asynchronous database query. Task result contains the desired portfolio DTO if it exists, null otherwise.</returns>
        Task<QueryResponse<PortfolioDto>> GetPortfolio(int portfolioId);

        /// <summary>
        /// Retrieves the value of the portfolio at the specified time.
        /// </summary>
        /// <param name="portfolioId">ID of the portfolio to retrieve value of.</param>
        /// <param name="time">Value time.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains the latest portfolio value DTO before the specified time if such exists, null otherwise.
        /// </returns>
        Task<QueryResponse<EntityValueDto>> GetPortfolioValue(int portfolioId, DateTime time);

        /// <summary>
        /// Calculates the portfolio's profit in the specified date range.
        /// </summary>
        /// <param name="portfolioId">ID of the portfolio to calculate profit of.</param>
        /// <param name="dateRange">Date range to calculate profit in.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains an <see cref="EntityProfitDto">EntityProfitDto</see> containing the portfolio's profit.
        /// </returns>
        Task<QueryResponse<EntityProfitDto>> GetPortfolioProfit(int portfolioId, DateRangeParams dateRange);

        /// <summary>
        /// Calculates the portfolio's performance in the specified date range.
        /// </summary>
        /// <param name="portfolioId">ID of the portfolio to calculate performance of.</param>
        /// <param name="dateRange">Date range to calculate performance in.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains an <see cref="EntityPerformanceDto">EntityPerformanceDto</see> containing the portfolio's performance.
        /// </returns>
        Task<QueryResponse<EntityPerformanceDto>> GetPortfolioPerformance(int portfolioId, DateRangeParams dateRange);

        /// <summary>
        /// Charts the portfolio's charted value in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="portfolioId">ID of the portfolio to calculate charted value of.</param>
        /// <param name="dateRange">Date range to calculate charted value in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <param name="targetCurrencyCode">Currency to chart portfolio values in.</param>
        /// /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains an <c>IEnumerable</c> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s
        /// containing the portfolio's value at the end of individual intervals.
        /// </returns>
        Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartPortfolioValue(int portfolioId,
            DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null);

        /// <summary>
        /// Charts the portfolio's profit in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="portfolioId">ID of the portfolio to calculate charted profit of.</param>
        /// <param name="dateRange">Date range to calculate charted profit in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <param name="targetCurrencyCode">Currency to chart portfolio profits in.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains an <c>IEnumerable</c> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s
        /// containing the portfolio's profit between the range start and ends of individual intervals.
        /// </returns>
        Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartPortfolioProfit(int portfolioId,
            DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null);

        /// <summary>
        /// Charts the portfolio's performance in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="portfolioId">ID of the portfolio to calculate charted performance of.</param>
        /// <param name="dateRange">Date range to calculate charted performance in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains an <c>IEnumerable</c> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s
        /// containing the portfolio's performance between the range start and ends of individual intervals.
        /// </returns>
        Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartPortfolioPerformance(int portfolioId,
            DateRangeParams dateRange, AggregationFrequency frequency);

        /// <summary>
        /// Charts the portfolio's aggregated profit in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="portfolioId">ID of the portfolio to calculate aggregated profit of.</param>
        /// <param name="dateRange">Date range to calculate aggregated profit in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <param name="targetCurrencyCode">Currency to chart portfolio profits in.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains an <c>IEnumerable</c> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s containing the portfolio's profit in individual intervals.
        /// </returns>
        Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartPortfolioProfitAggregated(int portfolioId,
            DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null);

        /// <summary>
        /// Charts the portfolio's aggregated performance in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="portfolioId">ID of the portfolio to calculate aggregated performance of.</param>
        /// <param name="dateRange">Date range to calculate aggregated performance in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains an <c>IEnumerable</c> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s
        /// containing the portfolio's performance in individual intervals.
        /// </returns>
        Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartPortfolioPerformanceAggregated(int portfolioId,
            DateRangeParams dateRange, AggregationFrequency frequency);
    }
}
