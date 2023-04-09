using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.Interfaces.Services
{
    /// <summary>
    /// Handles creation, modification and removal of portfolios.
    /// </summary>
    public interface IPortfolioService
    {
        /// <summary>
        /// Retrieves all portfolios ordered by ID.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains a <see cref="OperationResponse{T}"/> containing an <see cref="IEnumerable{T}"/> containing the retrieved portfolio DTOs.
        /// </returns>
        public Task<OperationResponse<IEnumerable<PortfolioDto>>> GetAllPortfoliosAsync();

        /// <summary>
        /// Retrieves a portfolio by ID.
        /// </summary>
        /// <param name="portfolioId">ID of the portfolio to retrieve.</param>
        /// <returns>A task representing the asynchronous retrieval operation. Task result contains a <see cref="OperationResponse{T}"/> containing  the desired portfolio DTO if it exists, null otherwise.</returns>
        public Task<OperationResponse<PortfolioDto>> GetPortfolioAsync(int portfolioId);

        /// <summary>
        /// Retrieves the value of the portfolio at the specified time.
        /// </summary>
        /// <param name="portfolioId">ID of the portfolio to retrieve value of.</param>
        /// <param name="time">Value time.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains a <see cref="OperationResponse{T}"/> containing the latest portfolio value DTO before the specified time if such exists, null otherwise.
        /// </returns>
        public Task<OperationResponse<EntityValueDto>> GetPortfolioValueAsync(int portfolioId, DateTime time);

        /// <summary>
        /// Calculates the portfolio's profit in the specified date range.
        /// </summary>
        /// <param name="portfolioId">ID of the portfolio to calculate profit of.</param>
        /// <param name="dateRange">Date range to calculate profit in.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains a <see cref="OperationResponse{T}"/> containing an <see cref="EntityProfitDto">EntityProfitDto</see> containing the portfolio's profit.
        /// </returns>
        public Task<OperationResponse<EntityProfitDto>> GetPortfolioProfitAsync(int portfolioId,
            DateRangeParams dateRange);

        /// <summary>
        /// Calculates the portfolio's performance in the specified date range.
        /// </summary>
        /// <param name="portfolioId">ID of the portfolio to calculate performance of.</param>
        /// <param name="dateRange">Date range to calculate performance in.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains a <see cref="OperationResponse{T}"/> containing an <see cref="EntityPerformanceDto">EntityPerformanceDto</see> containing the portfolio's performance.
        /// </returns>
        public Task<OperationResponse<EntityPerformanceDto>> GetPortfolioPerformanceAsync(int portfolioId,
            DateRangeParams dateRange);

        /// <summary>
        /// Charts the portfolio's charted value in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="portfolioId">ID of the portfolio to calculate charted value of.</param>
        /// <param name="dateRange">Date range to calculate charted value in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <param name="targetCurrencyCode">Currency to chart portfolio values in.</param>
        /// /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains a <see cref="OperationResponse{T}"/> containing an <see cref="IEnumerable{T}"/> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s
        /// containing the portfolio's value at the end of individual intervals.
        /// </returns>
        public Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartPortfolioValueAsync(int portfolioId,
            DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null);

        /// <summary>
        /// Charts the portfolio's profit in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="portfolioId">ID of the portfolio to calculate charted profit of.</param>
        /// <param name="dateRange">Date range to calculate charted profit in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <param name="targetCurrencyCode">Currency to chart portfolio profits in.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains a <see cref="OperationResponse{T}"/> containing an <see cref="IEnumerable{T}"/> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s
        /// containing the portfolio's profit between the range start and ends of individual intervals.
        /// </returns>
        public Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartPortfolioProfitAsync(int portfolioId,
            DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null);

        /// <summary>
        /// Charts the portfolio's performance in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="portfolioId">ID of the portfolio to calculate charted performance of.</param>
        /// <param name="dateRange">Date range to calculate charted performance in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains a <see cref="OperationResponse{T}"/> containing an <see cref="IEnumerable{T}"/> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s
        /// containing the portfolio's performance between the range start and ends of individual intervals.
        /// </returns>
        public Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartPortfolioPerformanceAsync(
            int portfolioId, DateRangeParams dateRange, AggregationFrequency frequency);

        /// <summary>
        /// Charts the portfolio's aggregated profit in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="portfolioId">ID of the portfolio to calculate aggregated profit of.</param>
        /// <param name="dateRange">Date range to calculate aggregated profit in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <param name="targetCurrencyCode">Currency to chart portfolio profits in.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains a <see cref="OperationResponse{T}"/> containing an <see cref="IEnumerable{T}"/> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s containing the portfolio's profit in individual intervals.
        /// </returns>
        public Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartPortfolioAggregatedProfitAsync(
            int portfolioId,
            DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null);

        /// <summary>
        /// Charts the portfolio's aggregated performance in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="portfolioId">ID of the portfolio to calculate aggregated performance of.</param>
        /// <param name="dateRange">Date range to calculate aggregated performance in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains a <see cref="OperationResponse{T}"/> containing an <see cref="IEnumerable{T}"/> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s
        /// containing the portfolio's performance in individual intervals.
        /// </returns>
        public Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartPortfolioAggregatedPerformanceAsync(
            int portfolioId, DateRangeParams dateRange, AggregationFrequency frequency);

        /// <summary>
        /// Retrieves performance and profit statistics of all portfolios for the past day, week, month, and total.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains a <see cref="OperationResponse{T}"/> containing an <see cref="IEnumerable{T}"/> off <see cref="EntityStatisticsDto"/>
        /// containing the portfolios' statistics.
        /// </returns>
        public Task<OperationResponse<IEnumerable<EntityStatisticsDto>>> GetAllPortfoliosStatisticsAsync();

        /// <summary>
        /// Retrieves performance and profit statistics of a given portfolio for the past day, week, month, and total.
        /// </summary>
        /// <param name="portfolioId">Portfolio ID</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains a <see cref="OperationResponse{T}"/> containing an <see cref="EntityStatisticsDto"/> containing the portfolio's statistics.
        /// </returns>
        public Task<OperationResponse<EntityStatisticsDto>> GetPortfolioStatisticsAsync(int portfolioId);

        /// <summary>
        /// Creates a portfolio according to the supplied DTO.
        /// </summary>
        /// <param name="options">A DTO containing client's request body.</param>
        /// <returns>A task representing the asynchronous creation operation. Task result contains an <see cref="OperationResponse{T}"/> containing the created portfolio.</returns>
        public Task<OperationResponse<PortfolioDto>> CreatePortfolioAsync(PortfolioDto options);

        /// <summary>
        /// Modifies a portfolio according to the supplied DTO.
        /// </summary>
        /// <param name="options">A DTO containing client's request body.</param>
        /// <returns>A task representing the asynchronous update operation. Task result contains an <see cref="OperationResponse{T}"/> containing the updated portfolio.</returns>
        public Task<OperationResponse<PortfolioDto>> UpdatePortfolioAsync(PortfolioDto options);

        /// <summary>
        /// Deletes a portfolio.
        /// </summary>
        /// <param name="id">Portfolio ID.</param>
        /// <returns>
        /// A task representing the asynchronous deletion operation.
        /// Task result contains an <see cref="OperationResponse"/> representing the status of the operation.
        /// </returns>
        public Task<OperationResponse> DeletePortfolioAsync(int id);
    }
}
