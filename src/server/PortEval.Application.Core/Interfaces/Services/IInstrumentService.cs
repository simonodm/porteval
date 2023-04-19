using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.Interfaces.Services;

/// <summary>
///     Handles creation and removal of instruments.
/// </summary>
public interface IInstrumentService
{
    /// <summary>
    ///     Retrieves all instruments ordered by symbol alphabetically.
    /// </summary>
    /// <returns>
    ///     A task representing the asynchronous retrieval operation.
    ///     Task result contains an <see cref="OperationResponse{T}" /> containing the retrieved instruments.
    /// </returns>
    public Task<OperationResponse<IEnumerable<InstrumentDto>>> GetAllInstrumentsAsync();

    /// <summary>
    ///     Retrieves all instruments ordered by symbol alphabetically. The result is paginated.
    /// </summary>
    /// <param name="pagination">Pagination parameters.</param>
    /// <returns>
    ///     A task representing the asynchronous retrieval operation.
    ///     Task result contains an <see cref="OperationResponse{T}" /> containing the retrieved instruments page.
    /// </returns>
    public Task<OperationResponse<PaginatedResponse<InstrumentDto>>> GetInstrumentsPageAsync(
        PaginationParams pagination);

    /// <summary>
    ///     Retrieves an instrument by ID.
    /// </summary>
    /// <param name="instrumentId">ID of the instrument to retrieve.</param>
    /// <returns>
    ///     A task representing the asynchronous retrieval operation.
    ///     Task result contains an <see cref="OperationResponse{T}" /> containing the desired instrument DTO.
    /// </returns>
    public Task<OperationResponse<InstrumentDto>> GetInstrumentAsync(int instrumentId);

    /// <summary>
    ///     Retrieves all known stock exchanges.
    /// </summary>
    /// <returns>
    ///     A task representing the asynchronous retrieval operation.
    ///     Task result contains an <see cref="OperationResponse{T}" /> containing an <see cref="IEnumerable{T}" /> containing
    ///     retrieved stock exchanges.
    /// </returns>
    public Task<OperationResponse<IEnumerable<ExchangeDto>>> GetKnownExchangesAsync();

    /// <summary>
    ///     Calculates the instrument's profit in the specified date range.
    /// </summary>
    /// <param name="instrumentId">ID of the instrument to calculate profit of.</param>
    /// <param name="dateRange">Date range to calculate profit in.</param>
    /// <returns>
    ///     A task representing the asynchronous retrieval operation.
    ///     Task result contains an <see cref="OperationResponse{T}" /> containing the instrument's profit.
    /// </returns>
    public Task<OperationResponse<EntityProfitDto>> GetInstrumentProfitAsync(int instrumentId,
        DateRangeParams dateRange);

    /// <summary>
    ///     Calculates the instrument's performance in the specified date range.
    /// </summary>
    /// <param name="instrumentId">ID of the instrument to calculate performance of.</param>
    /// <param name="dateRange">Date range to calculate performance in.</param>
    /// <returns>
    ///     A task representing the asynchronous retrieval operation.
    ///     Task result contains an <see cref="OperationResponse{T}" /> containing the instrument's performance.
    /// </returns>
    public Task<OperationResponse<EntityPerformanceDto>> GetInstrumentPerformanceAsync(int instrumentId,
        DateRangeParams dateRange);

    /// <summary>
    ///     Charts the instrument's charted prices in the specified date range with the specified frequency.
    /// </summary>
    /// <param name="instrumentId">ID of the instrument to chart prices of.</param>
    /// <param name="dateRange">Date range to chart prices in.</param>
    /// <param name="frequency">Aggregation frequency.</param>
    /// <param name="currencyCode">Currency to convert prices to.</param>
    /// <returns>
    ///     A task representing the asynchronous retrieval operation.
    ///     Task result contains an <see cref="OperationResponse{T}" /> containing an <see cref="IEnumerable{T}" /> of
    ///     <see cref="EntityChartPointDto">EntityChartPointDto</see>s
    ///     containing the instrument's prices from the date range start until the end of individual intervals.
    /// </returns>
    public Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentPricesAsync(int instrumentId,
        DateRangeParams dateRange, AggregationFrequency frequency, string currencyCode = null);

    /// <summary>
    ///     Charts the instrument's charted profit in the specified date range with the specified frequency.
    /// </summary>
    /// <param name="instrumentId">ID of the instrument to chart profit of.</param>
    /// <param name="dateRange">Date range to chart profit in.</param>
    /// <param name="frequency">Aggregation frequency.</param>
    /// <param name="currencyCode">Currency to convert profits to.</param>
    /// <returns>
    ///     A task representing the asynchronous retrieval operation.
    ///     Task result contains an <see cref="OperationResponse{T}" /> containing an <see cref="IEnumerable{T}" /> of
    ///     <see cref="EntityChartPointDto">EntityChartPointDto</see>s
    ///     containing the instrument's profit from the date range start until the end of individual intervals.
    /// </returns>
    public Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentProfitAsync(int instrumentId,
        DateRangeParams dateRange, AggregationFrequency frequency, string currencyCode = null);

    /// <summary>
    ///     Charts the instrument's charted performance in the specified date range with the specified frequency.
    /// </summary>
    /// <param name="instrumentId">ID of the instrument to chart performance of.</param>
    /// <param name="dateRange">Date range to chart performance in.</param>
    /// <param name="frequency">Aggregation frequency.</param>
    /// <returns>
    ///     A task representing the asynchronous retrieval operation.
    ///     Task result contains an <see cref="OperationResponse{T}" /> containing an <see cref="IEnumerable{T}" /> of
    ///     <see cref="EntityChartPointDto">EntityChartPointDto</see>s
    ///     containing the instrument's performance from the date range start until the end of individual intervals.
    /// </returns>
    public Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentPerformanceAsync(
        int instrumentId,
        DateRangeParams dateRange, AggregationFrequency frequency);

    /// <summary>
    ///     Charts the instrument's aggregated profit in the specified date range with the specified frequency.
    /// </summary>
    /// <param name="instrumentId">ID of the instrument to chart aggregated profit of.</param>
    /// <param name="dateRange">Date range to chart aggregated profit in.</param>
    /// <param name="frequency">Aggregation frequency.</param>
    /// <param name="currencyCode">Currency to convert profits to.</param>
    /// <returns>
    ///     A task representing the asynchronous retrieval operation.
    ///     Task result contains an <see cref="OperationResponse{T}" /> containing an <see cref="IEnumerable{T}" /> of
    ///     <see cref="EntityChartPointDto">EntityChartPointDto</see>s containing the instrument's profit in individual
    ///     intervals.
    /// </returns>
    public Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentAggregatedProfitAsync(
        int instrumentId,
        DateRangeParams dateRange, AggregationFrequency frequency, string currencyCode = null);

    /// <summary>
    ///     Charts the instrument's aggregated performance in the specified date range with the specified frequency.
    /// </summary>
    /// <param name="instrumentId">ID of the instrument to chart aggregated performance of.</param>
    /// <param name="dateRange">Date range to chart aggregated performance in.</param>
    /// <param name="frequency">Aggregation frequency.</param>
    /// <returns>
    ///     A task representing the asynchronous retrieval operation.
    ///     Task result contains an <see cref="OperationResponse{T}" /> containing an <see cref="IEnumerable{T}" /> of
    ///     <see cref="EntityChartPointDto">EntityChartPointDto</see>s containing the instrument's performance in individual
    ///     intervals.
    /// </returns>
    public Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentAggregatedPerformanceAsync(
        int instrumentId,
        DateRangeParams dateRange, AggregationFrequency frequency);

    /// <summary>
    ///     Creates an instrument according to the supplied DTO.
    /// </summary>
    /// <param name="options">A DTO containing instrument data.</param>
    /// <returns>
    ///     A task representing the asynchronous creation operation.
    ///     Task result contains an <see cref="OperationResponse{T}" /> containing the created instrument.
    /// </returns>
    public Task<OperationResponse<InstrumentDto>> CreateInstrumentAsync(InstrumentDto options);

    /// <summary>
    ///     Updates an instrument according to the supplied DTO.
    /// </summary>
    /// <param name="options">A DTO containing instrument data.</param>
    /// <returns>
    ///     A task representing the asynchronous update operation.
    ///     Task result contains an <see cref="OperationResponse{T}" /> containing the updated instrument.
    /// </returns>
    public Task<OperationResponse<InstrumentDto>> UpdateInstrumentAsync(InstrumentDto options);

    /// <summary>
    ///     Deletes an instrument.
    /// </summary>
    /// <param name="id">Instrument ID.</param>
    /// <returns>
    ///     A task representing the asynchronous deletion operation.
    ///     Task result contains an <see cref="OperationResponse" /> representing the status of the operation.
    /// </returns>
    public Task<OperationResponse> DeleteAsync(int id);
}