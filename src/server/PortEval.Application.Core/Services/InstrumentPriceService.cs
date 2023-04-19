using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Core.Extensions;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.Services;

/// <inheritdoc cref="IInstrumentPriceService" />
public class InstrumentPriceService : IInstrumentPriceService
{
    private readonly IInstrumentQueries _instrumentDataQueries;
    private readonly IInstrumentPriceRepository _instrumentPriceRepository;
    private readonly IInstrumentRepository _instrumentRepository;

    /// <summary>
    ///     Initializes the service.
    /// </summary>
    public InstrumentPriceService(IInstrumentRepository instrumentRepository,
        IInstrumentPriceRepository instrumentPriceRepository, IInstrumentQueries instrumentDataQueries)
    {
        _instrumentRepository = instrumentRepository;
        _instrumentPriceRepository = instrumentPriceRepository;
        _instrumentDataQueries = instrumentDataQueries;
    }

    /// <inheritdoc />
    public async Task<OperationResponse<IEnumerable<InstrumentPriceDto>>> GetInstrumentPricesAsync(int instrumentId,
        DateRangeParams dateRange)
    {
        if (!await _instrumentRepository.ExistsAsync(instrumentId))
            return new OperationResponse<IEnumerable<InstrumentPriceDto>>
            {
                Status = OperationStatus.NotFound,
                Message = $"Instrument {instrumentId} does not exist."
            };

        var prices =
            await _instrumentDataQueries.GetInstrumentPricesAsync(instrumentId, dateRange.From, dateRange.To);
        return new OperationResponse<IEnumerable<InstrumentPriceDto>>
        {
            Response = prices
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<IEnumerable<InstrumentPriceDto>>> GetInstrumentPricesAsync(
        InstrumentDto instrument, DateRangeParams dateRange)
    {
        var prices =
            await _instrumentDataQueries.GetInstrumentPricesAsync(instrument.Id, dateRange.From, dateRange.To);
        return new OperationResponse<IEnumerable<InstrumentPriceDto>>
        {
            Response = prices
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<PaginatedResponse<InstrumentPriceDto>>> GetInstrumentPricesPageAsync(
        int instrumentId,
        PaginationParams pagination,
        DateRangeParams dateRange,
        bool compressed = false,
        AggregationFrequency? frequency = null)
    {
        if (!await _instrumentRepository.ExistsAsync(instrumentId))
            return new OperationResponse<PaginatedResponse<InstrumentPriceDto>>
            {
                Status = OperationStatus.NotFound,
                Message = $"Instrument {instrumentId} does not exist."
            };

        var totalCount = compressed
            ? await _instrumentDataQueries.GetInstrumentPriceCompressedCountAsync(instrumentId, dateRange.From,
                dateRange.To,
                frequency)
            : await _instrumentDataQueries.GetInstrumentPriceCountAsync(instrumentId, dateRange.From, dateRange.To,
                frequency);
        var data = compressed
            ? await _instrumentDataQueries.GetInstrumentPricesPageCompressedAsync(instrumentId, dateRange.From,
                dateRange.To, pagination, frequency)
            : await _instrumentDataQueries.GetInstrumentPricesPageAsync(instrumentId, dateRange.From, dateRange.To,
                pagination, frequency);

        var prices = data.ToList();

        var paginatedResponse =
            new PaginatedResponse<InstrumentPriceDto>(pagination.Page, prices.Count, totalCount, prices);
        return new OperationResponse<PaginatedResponse<InstrumentPriceDto>>
        {
            Response = paginatedResponse
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<InstrumentPriceDto>> GetInstrumentPriceAsync(int instrumentId, DateTime time)
    {
        if (!await _instrumentRepository.ExistsAsync(instrumentId))
            return new OperationResponse<InstrumentPriceDto>
            {
                Status = OperationStatus.NotFound,
                Message = $"Instrument {instrumentId} does not exist."
            };

        var price = await _instrumentDataQueries.GetInstrumentPriceAsync(instrumentId, time);
        return new OperationResponse<InstrumentPriceDto>
        {
            Status = price != null ? OperationStatus.Ok : OperationStatus.NotFound,
            Message = price != null ? "" : $"No price found for instrument {instrumentId} at {time}.",
            Response = price
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<InstrumentPriceDto>> AddPricePointAsync(InstrumentPriceDto options)
    {
        var instrument = await _instrumentRepository.FindAsync(options.InstrumentId);
        if (instrument == null)
            return new OperationResponse<InstrumentPriceDto>
            {
                Status = OperationStatus.NotFound,
                Message = $"Instrument {options.InstrumentId} does not exist."
            };

        if (await _instrumentPriceRepository.ExistsAsync(options.InstrumentId, options.Time))
            return new OperationResponse<InstrumentPriceDto>
            {
                Status = OperationStatus.Error,
                Message = $"{instrument.Symbol} already contains a price at {options.Time}."
            };

        var pricePoint = InstrumentPrice.Create(options.Time.RoundDown(TimeSpan.FromMinutes(1)), options.Price,
            instrument);
        _instrumentPriceRepository.Add(pricePoint);
        await _instrumentPriceRepository.UnitOfWork.CommitAsync();

        return await GetInstrumentPriceAsync(instrument.Id, pricePoint.Time);
    }

    /// <inheritdoc />
    public async Task<OperationResponse> DeletePricePointByIdAsync(int instrumentId, int priceId)
    {
        if (!await _instrumentPriceRepository.ExistsAsync(instrumentId, priceId))
            return new OperationResponse
            {
                Status = OperationStatus.NotFound,
                Message = $"Price {priceId} does not exist on instrument {instrumentId}."
            };
        await _instrumentPriceRepository.DeleteAsync(instrumentId, priceId);
        await _instrumentPriceRepository.UnitOfWork.CommitAsync();
        return new OperationResponse();
    }
}