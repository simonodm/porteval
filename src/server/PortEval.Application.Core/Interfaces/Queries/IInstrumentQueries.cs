﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Queries;
using PortEval.Application.Models;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.Interfaces.Queries
{
    /// <summary>
    /// Implements high performance read-only instrument queries.
    /// </summary>
    public interface IInstrumentQueries
    {
        /// <summary>
        /// Retrieves all instruments ordered by symbol alphabetically.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <see cref="IEnumerable{T}"/> containing the retrieved instrument DTOs.
        /// </returns>
        Task<QueryResponse<IEnumerable<InstrumentDto>>> GetAllInstruments();

        /// <summary>
        /// Retrieves all instruments ordered by symbol alphabetically. The result is paginated.
        /// </summary>
        /// <param name="pagination">Pagination parameters.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over a <see cref="PaginatedResponse{TPayload}">PaginatedResponse</see> containing the retrieved instrument DTOs.
        /// </returns>
        Task<QueryResponse<PaginatedResponse<InstrumentDto>>> GetInstrumentsPage(PaginationParams pagination);

        /// <summary>
        /// Retrieves an instrument by ID.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to retrieve.</param>
        /// <returns>A task representing the asynchronous database query. Task result contains a <see cref="QueryResponse{T}"/> wrapper over the desired instrument DTO.</returns>
        Task<QueryResponse<InstrumentDto>> GetInstrument(int instrumentId);

        /// <summary>
        /// Retrieves all instrument prices in the specified date range ordered by price time descending.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to retrieve prices of.</param>
        /// <param name="dateRange">Date range to retrieve prices for.</param>
        /// <returns>A task representing the asynchronous database query. Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <c>IEnumerable</c> of price DTOs.</returns>
        Task<QueryResponse<IEnumerable<InstrumentPriceDto>>> GetInstrumentPrices(int instrumentId,
            DateRangeParams dateRange);

        /// <summary>
        /// Retrieves all instrument prices in the specified date range ordered by price time descending.
        /// </summary>
        /// <param name="instrument">Instrument to retrieve prices of.</param>
        /// <param name="dateRange">Date range to retrieve prices for.</param>
        /// <returns>A task representing the asynchronous database query. Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <c>IEnumerable</c> of price DTOs.</returns>
        /// <remarks>Using this overload instead of <see cref="GetInstrumentPrices(int,DateRangeParams)"/> allows the caller to avoid an instrument existence check inherent to that method.</remarks>
        Task<QueryResponse<IEnumerable<InstrumentPriceDto>>> GetInstrumentPrices(InstrumentDto instrument,
            DateRangeParams dateRange);

        /// <summary>
        /// Retrieves instrument prices in the specified date range ordered by price time, descending. The result is paginated.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to retrieve prices of.</param>
        /// <param name="pagination">Pagination parameters</param>
        /// <param name="dateRange">Date range to retrieve prices for.</param>
        /// <param name="compressed">Determines whether repeated prices should be excluded.</param>
        /// <param name="frequency">Desired interval between prices.</param>
        /// <returns>A task representing the asynchronous database query. Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <c>IEnumerable</c> of price DTOs.</returns>
        Task<QueryResponse<PaginatedResponse<InstrumentPriceDto>>> GetInstrumentPricesPage(int instrumentId, PaginationParams pagination,
            DateRangeParams dateRange, bool compressed = false, AggregationFrequency? frequency = null);

        /// <summary>
        /// Retrieves splits of an instrument ordered by their time descending.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to retrieve splits of.</param>
        /// <returns>A task representing the asynchronous database query. Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <c>IEnumerable</c> of instrument split DTOs.</returns>
        Task<QueryResponse<IEnumerable<InstrumentSplitDto>>> GetInstrumentSplits(int instrumentId);

        /// <summary>
        /// Retrieves a single split of an instrument.
        /// </summary>
        /// <param name="instrumentId">ID of the parent instrument.</param>
        /// <param name="splitId">ID of the split.</param>
        /// <returns>A task representing the asynchronous database query. Task result contains a <see cref="QueryResponse{T}"/> wrapper over a split DTO.</returns>
        Task<QueryResponse<InstrumentSplitDto>> GetInstrumentSplit(int instrumentId, int splitId);

        /// <summary>
        /// Retrieves the last available price of the instrument before or equal to the specified time.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to retrieve price of.</param>
        /// <param name="time">Price time.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over the latest instrument price DTO before the specified time.
        /// </returns>
        Task<QueryResponse<InstrumentPriceDto>> GetInstrumentPrice(int instrumentId, DateTime time);

        /// <summary>
        /// Calculates the instrument's profit in the specified date range.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to calculate profit of.</param>
        /// <param name="dateRange">Date range to calculate profit in.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <see cref="EntityProfitDto">EntityProfitDto</see> containing the instrument's profit.
        /// </returns>
        Task<QueryResponse<EntityProfitDto>> GetInstrumentProfit(int instrumentId, DateRangeParams dateRange);

        /// <summary>
        /// Calculates the instrument's performance in the specified date range.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to calculate performance of.</param>
        /// <param name="dateRange">Date range to calculate performance in.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <see cref="EntityPerformanceDto">EntityPerformanceDto</see> containing the instrument's performance.
        /// </returns>
        Task<QueryResponse<EntityPerformanceDto>> GetInstrumentPerformance(int instrumentId, DateRangeParams dateRange);

        /// <summary>
        /// Charts the instrument's charted prices in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to calculate charted prices of.</param>
        /// <param name="dateRange">Date range to calculate charted prices in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <param name="targetCurrencyCode">Currency to convert prices to.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <c>IEnumerable</c> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s
        /// containing the instrument's prices from the date range start until the end of individual intervals.
        /// </returns>
        Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentPrices(int instrumentId,
            DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null);

        /// <summary>
        /// Charts the instrument's charted profit in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to calculate charted profit of.</param>
        /// <param name="dateRange">Date range to calculate charted profit in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <param name="targetCurrencyCode">Currency to convert profits to.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <c>IEnumerable</c> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s
        /// containing the instrument's profit from the date range start until the end of individual intervals.
        /// </returns>
        Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentProfit(int instrumentId,
            DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null);

        /// <summary>
        /// Charts the instrument's charted performance in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to calculate charted performance of.</param>
        /// <param name="dateRange">Date range to calculate charted performance in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <c>IEnumerable</c> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s
        /// containing the instrument's performance from the date range start until the end of individual intervals.
        /// </returns>
        Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentPerformance(int instrumentId,
            DateRangeParams dateRange, AggregationFrequency frequency);

        /// <summary>
        /// Charts the instrument's aggregated profit in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to calculate aggregated profit of.</param>
        /// <param name="dateRange">Date range to calculate aggregated profit in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <param name="targetCurrencyCode">Currency to convert profits to.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <c>IEnumerable</c> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s containing the instrument's profit in individual intervals.
        /// </returns>
        Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentProfitAggregated(int instrumentId,
            DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null);

        /// <summary>
        /// Charts the instrument's aggregated performance in the specified date range with the specified frequency.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to calculate aggregated performance of.</param>
        /// <param name="dateRange">Date range to calculate aggregated performance in.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <c>IEnumerable</c> of <see cref="EntityChartPointDto">EntityChartPointDto</see>s containing the instrument's performance in individual intervals.
        /// </returns>
        Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentPerformanceAggregated(int instrumentId,
            DateRangeParams dateRange, AggregationFrequency frequency);
    }
}
