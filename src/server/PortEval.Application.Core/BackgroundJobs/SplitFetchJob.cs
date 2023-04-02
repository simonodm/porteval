using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PortEval.Application.Core.Interfaces;
using PortEval.Application.Core.Interfaces.BackgroundJobs;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs.Enums;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;

namespace PortEval.Application.Core.BackgroundJobs
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
            _logger.LogInformation("Instrument split fetch job started.");
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

            _logger.LogInformation("Instrument split fetch job finished.");
        }
    }
}
