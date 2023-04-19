using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Interfaces.Services;

/// <summary>
///     Handles creation and removal of instrument price splits.
/// </summary>
public interface IInstrumentSplitService
{
    /// <summary>
    ///     Retrieves splits of an instrument ordered by their time descending.
    /// </summary>
    /// <param name="instrumentId">ID of the instrument to retrieve splits of.</param>
    /// <returns>
    ///     A task representing the asynchronous database query.
    ///     Task result contains a <see cref="OperationResponse{T}" /> containing the retrieved splits.
    /// </returns>
    public Task<OperationResponse<IEnumerable<InstrumentSplitDto>>> GetInstrumentSplitsAsync(int instrumentId);

    /// <summary>
    ///     Retrieves a single split of an instrument.
    /// </summary>
    /// <param name="instrumentId">ID of the parent instrument.</param>
    /// <param name="splitId">ID of the split.</param>
    /// <returns>
    ///     A task representing the asynchronous database query.
    ///     Task result contains a <see cref="OperationResponse{T}" /> containing the retrieved split, if it exists.
    /// </returns>
    public Task<OperationResponse<InstrumentSplitDto>> GetInstrumentSplitAsync(int instrumentId, int splitId);

    /// <summary>
    ///     Creates a price split according to the provided DTO.
    /// </summary>
    /// <param name="options">A DTO containing split data.</param>
    /// <returns>
    ///     A task representing the asynchronous creation operation.
    ///     Task result contains an <see cref="OperationResponse{T}" /> containing the created split.
    /// </returns>
    public Task<OperationResponse<InstrumentSplitDto>> CreateSplitAsync(InstrumentSplitDto options);

    /// <summary>
    ///     Updates an instrument split.
    /// </summary>
    /// <param name="instrumentId">ID of the expected parent instrument</param>
    /// <param name="options">A DTO containing updated split data.</param>
    /// <returns>
    ///     A task representing the asynchronous update operation.
    ///     Task result contains an <see cref="OperationResponse{T}" /> containing the updated split, if it exists.
    /// </returns>
    public Task<OperationResponse<InstrumentSplitDto>> UpdateSplitAsync(int instrumentId, InstrumentSplitDto options);
}