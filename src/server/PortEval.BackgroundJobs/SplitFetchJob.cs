using Microsoft.Extensions.Logging;
using PortEval.Application.Features.Interfaces;
using PortEval.Application.Features.Interfaces.BackgroundJobs;
using PortEval.Application.Features.Interfaces.Repositories;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Models.DTOs.Enums;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.BackgroundJobs
{
    public class SplitFetchJob : ISplitFetchJob
    {
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly IInstrumentSplitRepository _splitRepository;
        private readonly IFinancialDataFetcher _priceFetcher;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;

        public SplitFetchJob(IInstrumentRepository instrumentRepository, IInstrumentSplitRepository splitRepository,
            IFinancialDataFetcher priceFetcher, ILoggerFactory loggerFactory, INotificationService notificationService)
        {
            _instrumentRepository = instrumentRepository;
            _splitRepository = splitRepository;
            _priceFetcher = priceFetcher;
            _notificationService = notificationService;
            _logger = loggerFactory.CreateLogger<SplitFetchJob>();
        }

        public async Task Run()
        {
            _logger.LogInformation($"Instrument split fetch job started at {DateTime.UtcNow}.");
            var instruments = await _instrumentRepository.ListAllAsync();

            foreach (var instrument in instruments)
            {
                if (instrument.TrackingStatus != InstrumentTrackingStatus.Tracked)
                {
                    continue;
                }

                var existingSplits = await _splitRepository.ListInstrumentSplitsAsync(instrument.Id);
                var startTime = existingSplits.LastOrDefault()?.Time ?? instrument.TrackingInfo.TrackedSince;

                var splits = await _priceFetcher.GetInstrumentSplits(instrument.Symbol, startTime, DateTime.UtcNow);
                if (splits == null)
                {
                    continue;
                }

                foreach (var split in splits)
                {
                    var splitRatio = new SplitRatio(split.Denominator, split.Numerator);
                    var newSplit = InstrumentSplit.Create(instrument, split.Time, splitRatio);
                    _splitRepository.Add(newSplit);
                    await _splitRepository.UnitOfWork.CommitAsync();
                    await _notificationService.SendNotificationAsync(
                        NotificationType.Info,
                        $"{instrument.Symbol} {splitRatio.Numerator}-for-{splitRatio.Denominator} split detected on {newSplit.Time.Date}."
                    );
                }
            }

            _logger.LogInformation($"Instrument split fetch job finished at {DateTime.UtcNow}.");
        }
    }
}
