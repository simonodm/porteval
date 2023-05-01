using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PortEval.Application.Core.BackgroundJobs.Helpers;
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

/// <inheritdoc cref="IInitialPriceFetchJob" />
public class InitialPriceFetchJob : InstrumentPriceFetchJobBase, IInitialPriceFetchJob
{
    private readonly IInstrumentRepository _instrumentRepository;
    private readonly ILogger _logger;
    private readonly INotificationService _notificationService;
    private readonly IInstrumentPriceRepository _priceRepository;

    /// <summary>
    ///     Initializes the job.
    /// </summary>
    public InitialPriceFetchJob(IInstrumentRepository instrumentRepository,
        IInstrumentPriceRepository instrumentPriceRepository,
        INotificationService notificationService, IFinancialDataFetcher fetcher, ILoggerFactory loggerFactory)
        : base(fetcher)
    {
        _instrumentRepository = instrumentRepository;
        _priceRepository = instrumentPriceRepository;
        _notificationService = notificationService;
        _logger = loggerFactory.CreateLogger(typeof(InitialPriceFetchJob));
    }

    /// <inheritdoc />
    public async Task RunAsync(int instrumentId)
    {
        _logger.LogInformation($"First price fetch for instrument {instrumentId}.");
        var instrument = await _instrumentRepository.FindAsync(instrumentId);
        if (instrument == null)
        {
            _logger.LogError($"No instrument with id {instrumentId} found.");
            return;
        }

        var fetchStart = DateTime.UtcNow;

        var prices = await FetchInstrumentPrices(instrument, fetchStart);

        if (prices.Count != 0)
        {
            var pricesWithMissingRangesFilled = PriceUtils.FillMissingRangePrices(
                prices,
                time => PriceUtils.GetInstrumentPriceInterval(fetchStart, time),
                prices[0].Time,
                fetchStart);
            await SavePrices(instrument, pricesWithMissingRangesFilled);
        }
        else
        {
            instrument.SetTrackingStatus(InstrumentTrackingStatus.Untracked);
            instrument.IncreaseVersion();
            _instrumentRepository.Update(instrument);
            await _instrumentRepository.UnitOfWork.CommitAsync();
        }

        _logger.LogInformation($"First price fetch for instrument {instrumentId} finished.");
        if (prices.Count > 0)
        {
            await _notificationService.SendNotificationAsync(NotificationType.NewDataAvailable,
                $"Price download finished for {instrument.Symbol}.");
        }
        else
        {
            await _notificationService.SendNotificationAsync(NotificationType.NewDataAvailable,
                $"No prices found for {instrument.Symbol}.");
        }
    }

    /// <summary>
    ///     Aggregates all fetched price response objects into a single <c>IEnumerable</c> of successfully retrieved price
    ///     points.
    /// </summary>
    /// <param name="dailyPricesResponse">Daily prices Response object.</param>
    /// <param name="hourlyPricesResponse">Hourly prices Response object.</param>
    /// <param name="latestPricesResponse">Last day prices Response object.</param>
    /// <returns>A list of all retrieved prices.</returns>
    private List<PricePoint> ConcatFetchedPrices(IEnumerable<PricePoint> dailyPricesResponse,
        IEnumerable<PricePoint> hourlyPricesResponse,
        IEnumerable<PricePoint> latestPricesResponse)
    {
        var fetchedPrices = new List<PricePoint>();

        fetchedPrices.AddRange(dailyPricesResponse);
        fetchedPrices.AddRange(hourlyPricesResponse);
        fetchedPrices.AddRange(latestPricesResponse);

        return fetchedPrices;
    }

    /// <summary>
    ///     Saves price points in the database.
    /// </summary>
    /// <param name="instrument">Instrument to save prices of.</param>
    /// <param name="prices">Prices to save.</param>
    /// <returns>A task representing the asynchronous price save operation.</returns>
    private async Task SavePrices(Instrument instrument, IEnumerable<PricePoint> prices)
    {
        var pricesToAdd = new List<InstrumentPrice>();
        var minTime = DateTime.UtcNow;
        foreach (var pricePoint in prices)
        {
            if (pricePoint.Price <= 0m)
            {
                continue;
            }

            pricesToAdd.Add(InstrumentPrice.Create(pricePoint.Time, pricePoint.Price, instrument));

            if (pricePoint.Time < minTime)
            {
                minTime = pricePoint.Time;
            }
        }

        instrument.SetTrackingFrom(minTime);
        instrument.TrackingInfo.Update(DateTime.UtcNow);
        instrument.SetTrackingStatus(InstrumentTrackingStatus.Tracked);
        instrument.IncreaseVersion();
        _instrumentRepository.Update(instrument);
        await _instrumentRepository.UnitOfWork.CommitAsync();
        await _priceRepository.BulkUpsertAsync(pricesToAdd);
    }

    private async Task<List<PricePoint>> FetchInstrumentPrices(Instrument instrument, DateTime endTime)
    {
        var fiveDaysCutoffDate = endTime - PriceUtils.FiveDays;
        var oneDayCutoffDate = endTime - PriceUtils.OneDay;

        var dailyPricesResponse = await FetchHistoricalDailyPrices(instrument, PortEvalConstants.FinancialDataStartTime,
            fiveDaysCutoffDate);
        var hourlyPricesResponse = await FetchIntradayPrices(instrument, fiveDaysCutoffDate, oneDayCutoffDate,
            IntradayInterval.OneHour);
        var latestPricesResponse = await FetchIntradayPrices(instrument, oneDayCutoffDate, endTime,
            IntradayInterval.FiveMinutes);

        var allFetchedPrices = ConcatFetchedPrices(dailyPricesResponse, hourlyPricesResponse,
            latestPricesResponse);

        return allFetchedPrices;
    }
}