using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Common;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.Interfaces.Services
{
    /// <summary>
    /// Handles creation, modification and removal of positions.
    /// </summary>
    public interface IPositionService
    {
        /// <summary>
        /// Retrieves all positions.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing an <see cref="IEnumerable{T}"/> containing the retrieved position DTOs;
        /// </returns>
        public Task<OperationResponse<IEnumerable<PositionDto>>> GetAllPositionsAsync();

        /// <summary>
        /// Retrieves all positions of the given portfolio ordered by ID.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing an <see cref="IEnumerable{T}"/> containing the retrieved position DTOs.
        /// </returns>
        public Task<OperationResponse<IEnumerable<PositionDto>>> GetPortfolioPositionsAsync(int portfolioId);

        /// <summary>
        /// Retrieves all positions of the given portfolio ordered by ID.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing an <see cref="IEnumerable{T}"/> containing the retrieved position DTOs.
        /// </returns>
        /// <remarks>Using this overload instead of <see cref="GetPortfolioPositionsAsync(int)"/> allows the caller to avoid a portfolio existence check inherent to that method.</remarks>
        public Task<OperationResponse<IEnumerable<PositionDto>>> GetPortfolioPositionsAsync(
            PortfolioDto portfolio);

        /// <summary>
        /// Retrieves a position by its ID.
        /// </summary>
        /// <param name="positionId">ID of the position to retrieve.</param>
        /// <returns>A task representing the asynchronous retrieval operation. Task result contains an <see cref="OperationResponse{T}"/> containing the desired position DTO if it exists, null otherwise.</returns>
        public Task<OperationResponse<PositionDto>> GetPositionAsync(int positionId);

        /// <summary>
        /// Retrieves the value of the position at the specified time.
        /// </summary>
        /// <param name="positionId">ID of the position to retrieve value of.</param>
        /// <param name="time">Value time.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing the latest position value DTO before the specified time if such exists, null otherwise.
        /// </returns>
        public Task<OperationResponse<EntityValueDto>> GetPositionValueAsync(int positionId, DateTime time);

        /// <summary>
        /// Calculates the position's profit in the specified date range.
        /// </summary>
        /// <param name="positionId">ID of the position to calculate profit of.</param>
        /// <param name="dateRange">Date range to calculate profit in.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing an <see cref="EntityProfitDto">EntityProfitDto</see> containing the position's profit.
        /// </returns>
        public Task<OperationResponse<EntityProfitDto>> GetPositionProfitAsync(int positionId,
            DateRangeParams dateRange);

        /// <summary>
        /// Calculates the position's performance in the specified date range.
        /// </summary>
        /// <param name="positionId">ID of the position to calculate performance of.</param>
        /// <param name="dateRange">Date range to calculate performance in.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing an <see cref="EntityPerformanceDto">EntityPerformanceDto</see> containing the position's performance.
        /// </returns>
        public Task<OperationResponse<EntityPerformanceDto>> GetPositionPerformanceAsync(int positionId,
            DateRangeParams dateRange);

        /// <summary>
        /// Calculates the position's break-even point at the specified time.
        /// </summary>
        /// <param name="positionId">ID of the position to calculate BEP of.</param>
        /// <param name="time">Time at which the BEP should be calculated.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing a <see cref="PositionBreakEvenPointDto"/> containing the position's BEP.
        /// </returns>
        public Task<OperationResponse<PositionBreakEvenPointDto>> GetPositionBreakEvenPointAsync(int positionId,
            DateTime time);

        /// <summary>
        /// Charts the position's value in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="positionId">ID of the position to calculate charted value of.</param>
        /// <param name="dateRange">Date range to calculate charted value in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <param name="targetCurrencyCode">Currency to chart values in.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing an <see cref="IEnumerable{T}"/> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s
        /// containing the position's value at the ends of individual intervals.
        /// </returns>
        public Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartPositionValueAsync(int positionId,
            DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null);

        /// <summary>
        /// Charts the position's profit in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="positionId">ID of the position to calculate charted profit of.</param>
        /// <param name="dateRange">Date range to calculate charted profit in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <param name="targetCurrencyCode">Currency to chart profits in.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing an <see cref="IEnumerable{T}"/> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s
        /// containing the position's profit between the range start and ends of individual intervals.
        /// </returns>
        public Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartPositionProfitAsync(int positionId,
            DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null);

        /// <summary>
        /// Charts the position's performance in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="positionId">ID of the position to calculate charted performance of.</param>
        /// <param name="dateRange">Date range to calculate charted performance in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing an <see cref="IEnumerable{T}"/> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s
        /// containing the position's performance between the range start and ends of individual intervals.
        /// </returns>
        public Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartPositionPerformanceAsync(
            int positionId,
            DateRangeParams dateRange, AggregationFrequency frequency);

        /// <summary>
        /// Charts the position's aggregated profit in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="positionId">ID of the position to calculate aggregated profit of.</param>
        /// <param name="dateRange">Date range to calculate aggregated profit in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <param name="targetCurrencyCode">Currency to chart profits in.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing an <see cref="IEnumerable{T}"/> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s containing the position's profit in individual intervals.
        /// </returns>
        public Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartPositionAggregatedProfitAsync(
            int positionId,
            DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null);

        /// <summary>
        /// Charts the position's aggregated performance in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="positionId">ID of the position to calculate aggregated performance of.</param>
        /// <param name="dateRange">Date range to calculate aggregated performance in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing an <see cref="IEnumerable{T}"/> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s containing the position's performance in individual intervals.
        /// </returns>
        public Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartPositionAggregatedPerformanceAsync(
            int positionId,
            DateRangeParams dateRange, AggregationFrequency frequency);

        /// <summary>
        /// Retrieves performance and profit statistics of positions of a given portfolio for the past day, week, month, and total.
        /// </summary>
        /// <param name="portfolioId">Parent portfolio ID</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing an <see cref="IEnumerable{T}"/> of <see cref="PositionStatisticsDto"/> containing the positions' statistics.
        /// </returns>
        public Task<OperationResponse<IEnumerable<PositionStatisticsDto>>> GetPortfolioPositionsStatisticsAsync(
            int portfolioId);

        /// <summary>
        /// Retrieves performance and profit statistics of a given position for the past day, week, month, and total.
        /// </summary>
        /// <param name="positionId">Position ID</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing a <see cref="PositionStatisticsDto"/> containing the position's statistics.
        /// </returns>
        public Task<OperationResponse<PositionStatisticsDto>> GetPositionStatisticsAsync(int positionId);

        /// <summary>
        /// Retrieves position's price data, which contains the position's transactions until the end of <paramref name="dateRange"/> and the position's
        /// instrument prices at the start and at the end of <paramref name="dateRange" />.
        /// </summary>
        /// <param name="position">Position to retrieve price data for.</param>
        /// <param name="dateRange">Time range in which to retrieve the data.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing a <see cref="PositionPriceRangeData" /> containing the position's transactions and prices.
        /// </returns>
        public Task<OperationResponse<PositionPriceRangeData>> GetPositionPriceRangeDataAsync(PositionDto position,
            DateRangeParams dateRange);

        /// <summary>
        /// Retrieves position's price data, which contains the position's transactions until the end of <paramref name="dateRange"/> and the position's
        /// instrument prices until the end of <paramref name="dateRange" />.
        /// </summary>
        /// <param name="position">Position to retrieve price data for.</param>
        /// <param name="dateRange">Time range in which to retrieve the data.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing a <see cref="PositionPriceListData" /> containing the position's transactions and prices.
        /// </returns>
        public Task<OperationResponse<PositionPriceListData>> GetPositionPriceListDataAsync(PositionDto position,
            DateRangeParams dateRange);

        /// <summary>
        /// Opens a new position according to the supplied DTO.
        /// </summary>
        /// <param name="options">A DTO containing client's request body.</param>
        /// <returns>A task representing the asynchronous creation operation. Task result contains an <see cref="OperationResponse{T}"/> containing the newly opened position.</returns>
        public Task<OperationResponse<PositionDto>> OpenPositionAsync(PositionDto options);

        /// <summary>
        /// Modifies a position according to the supplied DTO.
        /// </summary>
        /// <param name="options">A DTO containing client's request body.</param>
        /// <returns>A task representing the asynchronous update operation. Task result contains an <see cref="OperationResponse{T}"/> containing the updated position.</returns>
        public Task<OperationResponse<PositionDto>> UpdatePositionAsync(PositionDto options);

        /// <summary>
        /// Deletes a position.
        /// </summary>
        /// <param name="positionId">Position ID.</param>
        /// <returns>
        /// A task representing the asynchronous deletion operation.
        /// Task result contains an <see cref="OperationResponse"/> representing the status of the operation.
        /// </returns>
        public Task<OperationResponse> RemovePositionAsync(int positionId);
    }
}
