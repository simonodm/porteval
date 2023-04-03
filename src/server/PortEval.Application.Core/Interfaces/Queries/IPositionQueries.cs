using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.Queries;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.Interfaces.Queries
{
    /// <summary>
    /// Implements high performance read-only position queries.
    /// </summary>
    public interface IPositionQueries
    {
        /// <summary>
        /// Retrieves all positions.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <c>IEnumerable</c> containing the retrieved position DTOs;
        /// </returns>
        Task<QueryResponse<IEnumerable<PositionDto>>> GetAllPositions();

        /// <summary>
        /// Retrieves all positions of the given portfolio ordered by ID.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <c>IEnumerable</c> containing the retrieved position DTOs.
        /// </returns>
        Task<QueryResponse<IEnumerable<PositionDto>>> GetPortfolioPositions(int portfolioId);

        /// <summary>
        /// Retrieves all positions of the given portfolio ordered by ID.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <c>IEnumerable</c> containing the retrieved position DTOs.
        /// </returns>
        /// <remarks>Using this overload instead of <see cref="GetPortfolioPositions(int)"/> allows the caller to avoid a portfolio existence check inherent to that method.</remarks>
        Task<QueryResponse<IEnumerable<PositionDto>>> GetPortfolioPositions(PortfolioDto portfolio);

        /// <summary>
        /// Retrieves a position by its ID.
        /// </summary>
        /// <param name="positionId">ID of the position to retrieve.</param>
        /// <returns>A task representing the asynchronous database query. Task result contains a <see cref="QueryResponse{T}"/> wrapper over the desired position DTO if it exists, null otherwise.</returns>
        Task<QueryResponse<PositionDto>> GetPosition(int positionId);

        /// <summary>
        /// Retrieves the value of the position at the specified time.
        /// </summary>
        /// <param name="positionId">ID of the position to retrieve value of.</param>
        /// <param name="time">Value time.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over the latest position value DTO before the specified time if such exists, null otherwise.
        /// </returns>
        Task<QueryResponse<EntityValueDto>> GetPositionValue(int positionId, DateTime time);

        /// <summary>
        /// Calculates the position's profit in the specified date range.
        /// </summary>
        /// <param name="positionId">ID of the position to calculate profit of.</param>
        /// <param name="dateRange">Date range to calculate profit in.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <see cref="EntityProfitDto">EntityProfitDto</see> containing the position's profit.
        /// </returns>
        Task<QueryResponse<EntityProfitDto>> GetPositionProfit(int positionId, DateRangeParams dateRange);

        /// <summary>
        /// Calculates the position's performance in the specified date range.
        /// </summary>
        /// <param name="positionId">ID of the position to calculate performance of.</param>
        /// <param name="dateRange">Date range to calculate performance in.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <see cref="EntityPerformanceDto">EntityPerformanceDto</see> containing the position's performance.
        /// </returns>
        Task<QueryResponse<EntityPerformanceDto>> GetPositionPerformance(int positionId, DateRangeParams dateRange);

        /// <summary>
        /// Calculates the position's break-even point at the specified time.
        /// </summary>
        /// <param name="positionId">ID of the position to calculate BEP of.</param>
        /// <param name="time">Time at which the BEP should be calculated.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over a <see cref="PositionBreakEvenPointDto"/> containing the position's BEP.
        /// </returns>
        Task<QueryResponse<PositionBreakEvenPointDto>> GetPositionBreakEvenPoint(int positionId, DateTime time);

        /// <summary>
        /// Charts the position's value in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="positionId">ID of the position to calculate charted value of.</param>
        /// <param name="dateRange">Date range to calculate charted value in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <param name="targetCurrencyCode">Currency to chart values in.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <c>IEnumerable</c> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s
        /// containing the position's value at the ends of individual intervals.
        /// </returns>
        Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartPositionValue(int positionId,
            DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null);

        /// <summary>
        /// Charts the position's profit in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="positionId">ID of the position to calculate charted profit of.</param>
        /// <param name="dateRange">Date range to calculate charted profit in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <param name="targetCurrencyCode">Currency to chart profits in.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <c>IEnumerable</c> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s
        /// containing the position's profit between the range start and ends of individual intervals.
        /// </returns>
        Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartPositionProfit(int positionId,
            DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null);

        /// <summary>
        /// Charts the position's performance in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="positionId">ID of the position to calculate charted performance of.</param>
        /// <param name="dateRange">Date range to calculate charted performance in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <c>IEnumerable</c> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s
        /// containing the position's performance between the range start and ends of individual intervals.
        /// </returns>
        Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartPositionPerformance(int positionId,
            DateRangeParams dateRange, AggregationFrequency frequency);

        /// <summary>
        /// Charts the position's aggregated profit in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="positionId">ID of the position to calculate aggregated profit of.</param>
        /// <param name="dateRange">Date range to calculate aggregated profit in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <param name="targetCurrencyCode">Currency to chart profits in.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <c>IEnumerable</c> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s containing the position's profit in individual intervals.
        /// </returns>
        Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartPositionProfitAggregated(int positionId,
            DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null);

        /// <summary>
        /// Charts the position's aggregated performance in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="positionId">ID of the position to calculate aggregated performance of.</param>
        /// <param name="dateRange">Date range to calculate aggregated performance in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <c>IEnumerable</c> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s containing the position's performance in individual intervals.
        /// </returns>
        Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartPositionPerformanceAggregated(int positionId,
            DateRangeParams dateRange, AggregationFrequency frequency);

        /// <summary>
        /// Retrieves performance and profit statistics of positions of a given portfolio for the past day, week, month, and total.
        /// </summary>
        /// <param name="portfolioId">Parent portfolio ID</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <c>IEnumerable</c> of <see cref="PositionStatisticsDto"/> containing the positions' statistics.
        /// </returns>
        Task<QueryResponse<IEnumerable<PositionStatisticsDto>>> GetPortfolioPositionsStatistics(int portfolioId);

        /// <summary>
        /// Retrieves performance and profit statistics of a given position for the past day, week, month, and total.
        /// </summary>
        /// <param name="id">Position ID</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over a <see cref="PositionStatisticsDto"/> containing the position's statistics.
        /// </returns>
        Task<QueryResponse<PositionStatisticsDto>> GetPositionStatistics(int id);

        /// <summary>
        /// Retrieves position's price data, which contains the position's transactions until the end of <paramref name="dateRange"/> and the position's
        /// instrument prices at the start and at the end of <paramref name="dateRange" />.
        /// </summary>
        /// <param name="position">Position to retrieve price data for.</param>
        /// <param name="dateRange">Time range in which to retrieve the data.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over a <see cref="PositionPriceRangeData" /> containing the position's transactions and prices.
        /// </returns>
        Task<PositionPriceRangeData> GetPositionPriceRangeData(PositionDto position, DateRangeParams dateRange);

        /// <summary>
        /// Retrieves position's price data, which contains the position's transactions until the end of <paramref name="dateRange"/> and the position's
        /// instrument prices until the end of <paramref name="dateRange" />.
        /// </summary>
        /// <param name="position">Position to retrieve price data for.</param>
        /// <param name="dateRange">Time range in which to retrieve the data.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over a <see cref="PositionPriceListData" /> containing the position's transactions and prices.
        /// </returns>
        Task<PositionPriceListData> GetPositionPriceListData(PositionDto position, DateRangeParams dateRange);
    }
}
