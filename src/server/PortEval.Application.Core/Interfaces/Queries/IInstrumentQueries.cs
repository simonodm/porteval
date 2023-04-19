using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.Interfaces.Queries;

/// <summary>
///     Implements queries for instruments stored in the application's persistent storage.
/// </summary>
public interface IInstrumentQueries
{
    /// <summary>
    ///     Retrieves all known instruments.
    /// </summary>
    /// <returns>
    ///     A task representing the asynchronous query operation.
    ///     Task result contains an <see cref="IEnumerable{T}" /> containing stored instruments.
    /// </returns>
    Task<IEnumerable<InstrumentDto>> GetAllInstrumentsAsync();

    /// <summary>
    ///     Retrieves the total count of stored instruments.
    /// </summary>
    /// <returns>
    ///     A task representing the asynchronous query operation.
    ///     Task result contains the number of stored instruments.
    /// </returns>
    Task<int> GetInstrumentCountAsync();

    /// <summary>
    ///     Retrieves an instruments page.
    /// </summary>
    /// <param name="pagination">Pagination parameters.</param>
    /// <returns>
    ///     A task representing the asynchronous query operation.
    ///     Task result contains an <see cref="IEnumerable{T}" /> containing the queried page of instruments.
    /// </returns>
    Task<IEnumerable<InstrumentDto>> GetInstrumentPageAsync(PaginationParams pagination);

    /// <summary>
    ///     Retrieves an instrument by ID.
    /// </summary>
    /// <param name="instrumentId">ID of the instrument to retrieve.</param>
    /// <returns>
    ///     A task representing the asynchronous query operation.
    ///     Task result contains the retrieved instrument if it exists, <c>null</c> otherwise.
    /// </returns>
    Task<InstrumentDto> GetInstrumentAsync(int instrumentId);

    /// <summary>
    ///     Retrieves instrument prices in the specified date range.
    /// </summary>
    /// <param name="instrumentId">ID of the instrument to retrieve prices of.</param>
    /// <param name="from">The start date of prices to retrieve.</param>
    /// <param name="to">The end date of prices to retrieve.</param>
    /// <returns>
    ///     A task representing the asynchronous query operation.
    ///     Task result contains an <see cref="IEnumerable{T}" /> containing the retrieved instrument prices.
    /// </returns>
    Task<IEnumerable<InstrumentPriceDto>> GetInstrumentPricesAsync(int instrumentId, DateTime from,
        DateTime to);

    /// <summary>
    ///     Retrieves the total number of prices in the specified date range with the specified aggregation frequency.
    /// </summary>
    /// <param name="instrumentId">ID of the instrument to retrieve prices of.</param>
    /// <param name="from">The start date of prices to retrieve.</param>
    /// <param name="to">The end date of prices to retrieve.</param>
    /// <param name="frequency">Frequency by which to aggregate the prices.</param>
    /// <returns>
    ///     A task representing the asynchronous query operation.
    ///     Task result contains the number of instrument prices fulfilling the specified parameters.
    /// </returns>
    Task<int> GetInstrumentPriceCountAsync(int instrumentId, DateTime from, DateTime to,
        AggregationFrequency? frequency = null);

    /// <summary>
    ///     Retrieves an instrument prices page.
    /// </summary>
    /// <param name="instrumentId">ID of the instrument to retrieve prices of.</param>
    /// <param name="from">The start date of prices to retrieve.</param>
    /// <param name="to">The end date of prices to retrieve.</param>
    /// <param name="pagination">Pagination parameters.</param>
    /// <param name="frequency">Frequency by which to aggregate the retrieved prices.</param>
    /// <returns>
    ///     A task representing the asynchronous query operation.
    ///     Task result contains an <see cref="IEnumerable{T}" /> containing the queried page of instrument prices.
    /// </returns>
    Task<IEnumerable<InstrumentPriceDto>> GetInstrumentPricesPageAsync(
        int instrumentId,
        DateTime from,
        DateTime to,
        PaginationParams pagination,
        AggregationFrequency? frequency = null);

    /// <summary>
    ///     Retrieves the total number compressed instrument prices in the specified date range with the specified frequency.
    /// </summary>
    /// <param name="instrumentId">ID of the instrument to retrieve prices of.</param>
    /// <param name="from">The start date of prices to retrieve.</param>
    /// <param name="to">The end date of prices to retrieve.</param>
    /// <param name="frequency">Frequency by which to aggregate the prices.</param>
    /// <returns>
    ///     A task representing the asynchronous query operation.
    ///     Task result contains the total number of compressed instrument prices fulfilling the specified parameters.
    /// </returns>
    Task<int> GetInstrumentPriceCompressedCountAsync(int instrumentId,
        DateTime from, DateTime to,
        AggregationFrequency? frequency = null);

    /// <summary>
    ///     Retrieves a compressed instrument prices page.
    /// </summary>
    /// <param name="instrumentId">ID of the instrument to retrieve prices of.</param>
    /// <param name="from">The start date of prices to retrieve.</param>
    /// <param name="to">The end date of prices to retrieve.</param>
    /// <param name="pagination">Pagination parameters.</param>
    /// <param name="frequency">Frequency by which to aggregate the retrieved prices.</param>
    /// <returns>
    ///     A task representing the asynchronous query operation.
    ///     Task result contains an <see cref="IEnumerable{T}" /> containing the queried page of compressed instrument prices.
    /// </returns>
    /// <remarks>Compressed instrument prices are prices filtered in such a way, that no consecutive price is the same.</remarks>
    Task<IEnumerable<InstrumentPriceDto>> GetInstrumentPricesPageCompressedAsync(int instrumentId,
        DateTime from, DateTime to,
        PaginationParams pagination, AggregationFrequency? frequency = null);

    /// <summary>
    ///     Retrieves an instrument price at the specified time.
    /// </summary>
    /// <param name="instrumentId">ID of the instrument to retrieve price of.</param>
    /// <param name="time">Time at which to retrieve the price.</param>
    /// <returns>
    ///     A task representing the asynchronous query operation.
    ///     Task result contains the latest instrument price earlier than <paramref name="time" /> if it exists, <c>null</c>
    ///     otherwise.
    /// </returns>
    Task<InstrumentPriceDto> GetInstrumentPriceAsync(int instrumentId, DateTime time);

    /// <summary>
    ///     Retrieves all splits of an instrument.
    /// </summary>
    /// <param name="instrumentId">ID of the instrument to retrieve splits of.</param>
    /// <returns>
    ///     A task representing the asynchronous query operation.
    ///     Task result contains all splits of the specified instrument.
    /// </returns>
    Task<IEnumerable<InstrumentSplitDto>> GetInstrumentSplitsAsync(int instrumentId);

    /// <summary>
    ///     Retrieves an instrument split by ID.
    /// </summary>
    /// <param name="instrumentId">ID of the instrument to retrieve split of.</param>
    /// <param name="splitId">ID of the split.</param>
    /// <returns>
    ///     A task representing the asynchronous query operation.
    ///     Task result contains the retrieved split if it exists, <c>null</c> otherwise.
    /// </returns>
    Task<InstrumentSplitDto> GetInstrumentSplitAsync(int instrumentId, int splitId);
}