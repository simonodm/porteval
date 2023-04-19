using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PortEval.Application.Core.BackgroundJobs.Helpers;
using PortEval.Application.Core.Extensions;
using PortEval.Application.Core.Interfaces;
using PortEval.Application.Core.Interfaces.BackgroundJobs;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs.Enums;
using PortEval.Application.Models.FinancialDataFetcher;
using PortEval.Domain;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.BackgroundJobs;

/// <inheritdoc cref="IMissingInstrumentPricesFetchJob" />
public class MissingInstrumentPricesFetchJob : InstrumentPriceFetchJobBase, IMissingInstrumentPricesFetchJob
{
    private readonly IInstrumentRepository _instrumentRepository;
    private readonly ILogger _logger;
    private readonly INotificationService _notificationService;
    private readonly IInstrumentPriceRepository _priceRepository;

    /// <summary>
    ///     Initializes the job.
    /// </summary>
    public MissingInstrumentPricesFetchJob(IInstrumentRepository instrumentRepository,
        IInstrumentPriceRepository instrumentPriceRepository,
        INotificationService notificationService, IFinancialDataFetcher fetcher, ILoggerFactory loggerFactory)
        : base(fetcher)
    {
        _instrumentRepository = instrumentRepository;
        _priceRepository = instrumentPriceRepository;
        _notificationService = notificationService;
        _logger = loggerFactory.CreateLogger(typeof(MissingInstrumentPricesFetchJob));
    }

    /// <inheritdoc />
    public async Task RunAsync()
    {
        var currentTime = DateTime.UtcNow;
        _logger.LogInformation("Starting missing prices fetch.");

        var instruments = await _instrumentRepository.ListAllAsync();

        foreach (var instrument in instruments)
        {
            var missingRanges = await GetMissingRanges(instrument, currentTime);

            foreach (var range in missingRanges)
            {
                var processedPrices = await ProcessMissingRange(instrument, range, currentTime);
                AdjustInstrumentTrackingBasedOnProcessedPrices(instrument, processedPrices);
            }

            if (instrument.TrackingInfo != null)
            {
                instrument.TrackingInfo.Update(currentTime);
                instrument.IncreaseVersion();
                _instrumentRepository.Update(instrument);
            }
        }

        await _instrumentRepository.UnitOfWork.CommitAsync();
        _logger.LogInformation("Missing prices fetch finished.");
        await _notificationService.SendNotificationAsync(NotificationType.NewDataAvailable);
    }

    private async Task<IEnumerable<TimeRange>> GetMissingRanges(Instrument instrument, DateTime baseTime)
    {
        var prices = await _priceRepository.ListInstrumentPricesAsync(instrument.Id);
        var priceTimes = prices
            .Where(p => instrument.TrackingInfo == default || p.Time >= instrument.TrackingInfo.StartTime)
            .OrderBy(p => p.Time)
            .Select(p => p.Time);

        var missingRanges = PriceUtils.GetMissingPriceRanges(
            priceTimes,
            time => PriceUtils.GetInstrumentPriceInterval(baseTime, time),
            PortEvalConstants.FinancialDataStartTime,
            baseTime.RoundDown(PriceUtils.FiveMinutes)
        );

        return missingRanges;
    }

    /// <summary>
    ///     Retrieves and saves prices of a single instrument in the given time range.
    /// </summary>
    /// <param name="instrument">Instrument to retrieve prices for.</param>
    /// <param name="range">Time range of missing prices.</param>
    /// <param name="baseTime">Time used to determine the appropriate price intervals.</param>
    /// <returns>
    ///     A task representing the asynchronous retrieval and save operations.
    ///     Task result contains saved prices.
    /// </returns>
    private async Task<List<InstrumentPrice>> ProcessMissingRange(Instrument instrument, TimeRange range,
        DateTime baseTime)
    {
        var fetchResult = await FetchPricesInRange(instrument, range);

        var filledPrices = await FillMissingPrices(fetchResult, instrument, range, baseTime);
        var pricesToInsert = ProcessPricePoints(filledPrices, instrument, range);

        await _priceRepository.BulkUpsertAsync(pricesToInsert);

        return pricesToInsert;
    }

    private async Task<IEnumerable<PricePoint>> FetchPricesInRange(Instrument instrument, TimeRange range)
    {
        if (range.Interval < PriceUtils.OneDay)
        {
            var intradayInterval = range.Interval <= PriceUtils.FiveMinutes
                ? IntradayInterval.FiveMinutes
                : IntradayInterval.OneHour;
            return await FetchIntradayPrices(instrument, range.From, range.To,
                intradayInterval);
        }

        return await FetchHistoricalDailyPrices(instrument, range.From, range.To);
    }

    private async Task<IEnumerable<PricePoint>> FillMissingPrices(IEnumerable<PricePoint> prices,
        Instrument instrument, TimeRange range, DateTime baseTime)
    {
        var pricesWithStartingPricePrepended = prices;
        var priceAtRangeStart = await _priceRepository.FindPriceAtAsync(instrument.Id, range.From);

        if (priceAtRangeStart != null)
        {
            var rangeStartPricePoint = new PricePoint
            {
                Price = priceAtRangeStart.Price,
                CurrencyCode = instrument.CurrencyCode,
                Symbol = instrument.Symbol,
                Time = range.From
            };
            pricesWithStartingPricePrepended = prices.Prepend(rangeStartPricePoint);
        }


        var newPricePoints = PriceUtils.FillMissingRangePrices(
            pricesWithStartingPricePrepended,
            time => PriceUtils.GetInstrumentPriceInterval(baseTime, time),
            range.From,
            range.To);

        return newPricePoints;
    }

    private List<InstrumentPrice> ProcessPricePoints(IEnumerable<PricePoint> prices,
        Instrument instrument, TimeRange range)
    {
        var pricesToAdd = new List<InstrumentPrice>();
        foreach (var pricePoint in prices)
        {
            if (!((pricePoint.Time > range.From) & (pricePoint.Time < range.To)) || pricePoint.Price <= 0m) continue;

            pricesToAdd.Add(InstrumentPrice.Create(pricePoint.Time, pricePoint.Price, instrument));
        }

        return pricesToAdd;
    }

    private void AdjustInstrumentTrackingBasedOnProcessedPrices(Instrument instrument,
        List<InstrumentPrice> processedPrices)
    {
        if (processedPrices.Count == 0) return;

        if (instrument.TrackingInfo == null || instrument.TrackingInfo.StartTime > processedPrices[0].Time)
            instrument.SetTrackingFrom(processedPrices[0].Time);

        if (instrument.TrackingStatus != InstrumentTrackingStatus.Tracked)
            instrument.SetTrackingStatus(InstrumentTrackingStatus.Tracked);
    }
}