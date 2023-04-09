using System.Collections.Generic;
using PortEval.Application.Models.DTOs;
using System.Threading.Tasks;
using PortEval.Application.Models.QueryParams;
using PortEval.Application.Models;
using PortEval.Domain.Models.Enums;
using System;

namespace PortEval.Application.Core.Interfaces.Services
{
    /// <summary>
    /// Handles creation and deletion of instrument prices.
    /// </summary>
    public interface IInstrumentPriceService
    {
        /// <summary>
        /// Retrieves prices of an instrument in the given date range ordered by price time descending.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to retrieve prices of.</param>
        /// <param name="dateRange">Date range.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing the retrieved prices.
        /// </returns>
        public Task<OperationResponse<IEnumerable<InstrumentPriceDto>>> GetInstrumentPricesAsync(int instrumentId,
            DateRangeParams dateRange);

        /// <summary>
        /// Retrieves prices of an instrument in the given date range ordered by price time descending.
        /// </summary>
        /// <param name="instrument">Instrument to retrieve prices of.</param>
        /// <param name="dateRange">Date range.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing the retrieved prices.
        /// </returns>
        /// <remarks>Using this overload instead of <see cref="GetInstrumentPricesAsync(int,DateRangeParams)"/> allows the caller to avoid an instrument existence check inherent to that method.</remarks>
        public Task<OperationResponse<IEnumerable<InstrumentPriceDto>>> GetInstrumentPricesAsync(
            InstrumentDto instrument, DateRangeParams dateRange);

        /// <summary>
        /// Retrieves instrument prices in the specified date range ordered by price time, descending. The result is paginated.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to retrieve prices of.</param>
        /// <param name="pagination">Pagination parameters</param>
        /// <param name="dateRange">Date range to retrieve prices for.</param>
        /// <param name="compressed">Determines whether repeated prices should be excluded.</param>
        /// <param name="frequency">Desired interval between prices.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains a <see cref="OperationResponse{T}"/> containing a paginated response of the retrieved prices.
        /// </returns>
        public Task<OperationResponse<PaginatedResponse<InstrumentPriceDto>>> GetInstrumentPricesPageAsync(
            int instrumentId,
            PaginationParams pagination,
            DateRangeParams dateRange,
            bool compressed = false,
            AggregationFrequency? frequency = null);

        /// <summary>
        /// Retrieves the last available price of the instrument before or equal to the specified time.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to retrieve price of.</param>
        /// <param name="time">Price time.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="OperationResponse{T}"/> containing the retrieved price, if it exists.
        /// </returns>
        public Task<OperationResponse<InstrumentPriceDto>> GetInstrumentPriceAsync(int instrumentId, DateTime time);

        /// <summary>
        /// Adds an instrument price according to the supplied DTO.
        /// </summary>
        /// <param name="options">A DTO containing client's request body.</param>
        /// <returns>
        /// A task representing the asynchronous creation operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing the created price.
        /// </returns>
        public Task<OperationResponse<InstrumentPriceDto>> AddPricePointAsync(InstrumentPriceDto options);

        /// <summary>
        /// Deletes an instrument price.
        /// </summary>
        /// <param name="instrumentId">Parent instrument ID.</param>
        /// <param name="priceId">Instrument price ID.</param>
        /// <returns>
        /// A task representing the asynchronous deletion operation.
        /// Task result contains an <see cref="OperationResponse"/> representing the status of the operation.
        /// </returns>
        public Task<OperationResponse> DeletePricePointByIdAsync(int instrumentId, int priceId);
    }
}
