using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;

namespace PortEval.Application.Core.Services;

/// <inheritdoc cref="IInstrumentSplitService" />
public class InstrumentSplitService : IInstrumentSplitService
{
    private readonly IInstrumentQueries _instrumentDataQueries;
    private readonly IInstrumentRepository _instrumentRepository;
    private readonly IInstrumentSplitRepository _splitRepository;

    /// <summary>
    ///     Initializes the service.
    /// </summary>
    public InstrumentSplitService(IInstrumentRepository instrumentRepository,
        IInstrumentSplitRepository splitRepository, IInstrumentQueries instrumentDataQueries)
    {
        _instrumentRepository = instrumentRepository;
        _splitRepository = splitRepository;
        _instrumentDataQueries = instrumentDataQueries;
    }

    /// <inheritdoc />
    public async Task<OperationResponse<IEnumerable<InstrumentSplitDto>>> GetInstrumentSplitsAsync(int instrumentId)
    {
        var instrument = await _instrumentDataQueries.GetInstrumentAsync(instrumentId);
        if (instrument == null)
        {
            return new OperationResponse<IEnumerable<InstrumentSplitDto>>
            {
                Status = OperationStatus.NotFound,
                Message = $"Instrument {instrumentId} does not exist."
            };
        }

        var splits = await _instrumentDataQueries.GetInstrumentSplitsAsync(instrumentId);
        return new OperationResponse<IEnumerable<InstrumentSplitDto>>
        {
            Response = splits
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<InstrumentSplitDto>> GetInstrumentSplitAsync(int instrumentId, int splitId)
    {
        var instrument = await _instrumentDataQueries.GetInstrumentAsync(instrumentId);
        if (instrument == null)
        {
            return new OperationResponse<InstrumentSplitDto>
            {
                Status = OperationStatus.NotFound,
                Message = $"Instrument {instrumentId} does not exist."
            };
        }

        var split = await _instrumentDataQueries.GetInstrumentSplitAsync(instrumentId, splitId);
        return new OperationResponse<InstrumentSplitDto>
        {
            Status = split != null ? OperationStatus.Ok : OperationStatus.NotFound,
            Message = split != null ? "" : $"Split {splitId} does not exist on instrument {instrumentId}.",
            Response = split
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<InstrumentSplitDto>> CreateSplitAsync(InstrumentSplitDto options)
    {
        var instrument = await _instrumentRepository.FindAsync(options.InstrumentId);
        if (instrument == null)
        {
            return new OperationResponse<InstrumentSplitDto>
            {
                Status = OperationStatus.NotFound,
                Message = $"Instrument {options.InstrumentId} does not exist."
            };
        }

        var ratio = new SplitRatio(options.SplitRatioDenominator, options.SplitRatioNumerator);
        var newSplit = InstrumentSplit.Create(instrument, options.Time, ratio);

        _splitRepository.Add(newSplit);
        await _splitRepository.UnitOfWork.CommitAsync();

        return await GetInstrumentSplitAsync(instrument.Id, newSplit.Id);
    }

    /// <inheritdoc />
    public async Task<OperationResponse<InstrumentSplitDto>> UpdateSplitAsync(int instrumentId,
        InstrumentSplitDto options)
    {
        var split = await _splitRepository.FindAsync(options.Id);
        if (split == null)
        {
            return new OperationResponse<InstrumentSplitDto>
            {
                Status = OperationStatus.NotFound,
                Message = $"Split {options.Id} does not exist."
            };
        }

        if (split.InstrumentId != instrumentId)
        {
            return new OperationResponse<InstrumentSplitDto>
            {
                Status = OperationStatus.Error,
                Message = $"Split {options.Id} does not belong to instrument {instrumentId}."
            };
        }

        if (split.ProcessingStatus == InstrumentSplitProcessingStatus.Processed &&
            options.Status == InstrumentSplitProcessingStatus.RollbackRequested)
        {
            split.Rollback();
            split.IncreaseVersion();
            _splitRepository.Update(split);
            await _splitRepository.UnitOfWork.CommitAsync();
        }

        return await GetInstrumentSplitAsync(instrumentId, split.Id);
    }
}