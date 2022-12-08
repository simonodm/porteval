using PortEval.Domain.Models.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Exceptions;
using PortEval.FinancialDataFetcher;
using PortEval.FinancialDataFetcher.Models;
using System;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs.Enums;
using PortEval.Infrastructure;
using PortEval.Application.Services.Extensions;
using PortEval.Application.Services.Interfaces;
using PortEval.Application.Services.Interfaces.BackgroundJobs;
using PortEval.FinancialDataFetcher.Interfaces;

namespace PortEval.BackgroundJobs.LatestPricesFetch
{
    /// <summary>
    /// Retrieves the latest available prices of all existing instruments. Each price gets rounded down to the nearest 5 minutes.
    /// </summary>
    public class LatestPricesFetchJob : ILatestPricesFetchJob
    {
        private readonly IPriceFetcher _fetcher;
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly IInstrumentPriceRepository _instrumentPriceRepository;
        private readonly ICurrencyExchangeRateRepository _exchangeRateRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;

        public LatestPricesFetchJob(IPriceFetcher fetcher, IInstrumentRepository instrumentRepository,
            IInstrumentPriceRepository instrumentPriceRepository, ICurrencyExchangeRateRepository exchangeRateRepository,
            INotificationService notificationService, ILoggerFactory loggerFactory)
        {
            _fetcher = fetcher;
            _instrumentRepository = instrumentRepository;
            _instrumentPriceRepository = instrumentPriceRepository;
            _exchangeRateRepository = exchangeRateRepository;
            _notificationService = notificationService;
            _logger = loggerFactory.CreateLogger(typeof(LatestPricesFetchJob));
        }

        /// <summary>
        /// Starts the job.
        /// </summary>
        /// <returns>A task representing the asynchronous job processing operation.</returns>
        public async Task Run()
        {
            var startTime = DateTime.UtcNow;
            _logger.LogInformation($"Running latest price fetch job at {startTime}.");
            var instruments = await _instrumentRepository.ListAllAsync();

            foreach (var instrument in instruments)
            {
                if (instrument.IsTracked)
                {
                    var fetcherResponse = await _fetcher.GetLatestInstrumentPrice(instrument);
                    if (fetcherResponse.StatusCode != StatusCode.Ok || fetcherResponse.Result is null) continue;

                    try
                    {
                        var pricePoint = fetcherResponse.Result;
                        var price = await PriceUtils.GetConvertedPricePointPrice(_exchangeRateRepository, instrument,
                            pricePoint);
                        _instrumentPriceRepository.Add(new InstrumentPrice(startTime.RoundDown(TimeSpan.FromMinutes(5)), price, instrument.Id));

                        instrument.TrackingInfo.Update(startTime);
                        _instrumentRepository.Update(instrument);
                    }
                    catch (OperationNotAllowedException ex)
                    {
                        _logger.LogError(ex.Message);
                    }
                }
                
            }

            await _instrumentRepository.UnitOfWork.CommitAsync();
            _logger.LogInformation($"Finished latest price fetch job at {DateTime.UtcNow}.");
            await _notificationService.SendNotificationAsync(NotificationType.NewDataAvailable);
        }
    }
}
