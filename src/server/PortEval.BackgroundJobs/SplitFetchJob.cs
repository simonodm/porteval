using PortEval.Application.Features.Interfaces.BackgroundJobs;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PortEval.Application.Features.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;
using PortEval.FinancialDataFetcher.Interfaces;

namespace PortEval.BackgroundJobs
{
    public class SplitFetchJob : ISplitFetchJob
    {
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly IInstrumentSplitRepository _splitRepository;
        private readonly IPriceFetcher _priceFetcher;
        private readonly ILogger _logger;

        public SplitFetchJob(IInstrumentRepository instrumentRepository, IInstrumentSplitRepository splitRepository, IPriceFetcher priceFetcher, ILoggerFactory loggerFactory)
        {
            _instrumentRepository = instrumentRepository;
            _splitRepository = splitRepository;
            _priceFetcher = priceFetcher;
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
                if (splits.Result == null)
                {
                    continue;
                }

                foreach (var split in splits.Result)
                {
                    var splitRatio = new SplitRatio(split.Denominator, split.Numerator);
                    var newSplit = InstrumentSplit.Create(instrument, split.Time, splitRatio);
                    _splitRepository.Add(newSplit);
                    await _splitRepository.UnitOfWork.CommitAsync();
                }
            }

            _logger.LogInformation($"Instrument split fetch job finished at {DateTime.UtcNow}.");
        }
    }
}
