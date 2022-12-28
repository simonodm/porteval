using Microsoft.Extensions.Logging;
using PortEval.Application.Features.Interfaces.BackgroundJobs;
using PortEval.Application.Features.Interfaces.Repositories;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Models.DTOs.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.BackgroundJobs
{
    public class SplitPriceAndTransactionAdjustmentJob : ISplitPriceAndTransactionAdjustmentJob
    {
        private readonly IInstrumentSplitRepository _splitRepository;
        private readonly IInstrumentPriceRepository _priceRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;

        public SplitPriceAndTransactionAdjustmentJob(IInstrumentSplitRepository splitRepository,
            INotificationService notificationService, ILoggerFactory loggerFactory, IInstrumentPriceRepository priceRepository,
            IPositionRepository positionRepository)
        {
            _splitRepository = splitRepository;
            _notificationService = notificationService;
            _priceRepository = priceRepository;
            _positionRepository = positionRepository;
            _logger = loggerFactory.CreateLogger<SplitPriceAndTransactionAdjustmentJob>();
        }

        public async Task Run()
        {
            _logger.LogInformation($"Split price and transaction adjustment job started at {DateTime.UtcNow}.");
            var nonProcessedSplits = await _splitRepository.ListNonProcessedSplitsAsync();
            var rolledBackSplits = await _splitRepository.ListRollbackRequestedSplitsAsync();

            foreach (var split in nonProcessedSplits)
            {
                await AdjustPreSplitPrices(split.InstrumentId, split.Time, split.SplitRatio.Denominator, split.SplitRatio.Numerator);
                await AdjustPreSplitTransactions(split.InstrumentId, split.Time, split.SplitRatio.Denominator, split.SplitRatio.Numerator);
                split.MarkAsProcessed();
                split.IncreaseVersion();
                _splitRepository.Update(split);
            }

            foreach (var split in rolledBackSplits)
            {
                await AdjustPreSplitPrices(split.InstrumentId, split.Time, split.SplitRatio.Numerator, split.SplitRatio.Denominator);
                await AdjustPreSplitTransactions(split.InstrumentId, split.Time, split.SplitRatio.Numerator, split.SplitRatio.Denominator);
                split.MarkAsRolledBack();
                split.IncreaseVersion();
                _splitRepository.Update(split);
            }

            await _splitRepository.UnitOfWork.CommitAsync();

            if (nonProcessedSplits.Any())
            {
                await _notificationService.SendNotificationAsync(
                    NotificationType.NewDataAvailable,
                    "Prices and transactions have been adjusted following an instrument split."
                );
            }

            if (rolledBackSplits.Any())
            {
                await _notificationService.SendNotificationAsync(
                    NotificationType.NewDataAvailable,
                    "Prices and transactions have been adjusted following an instrument split rollback."
                );
            }

            _logger.LogInformation("Split price and transaction adjustment job finished.");
        }

        private async Task AdjustPreSplitPrices(int instrumentId, DateTime time, int before, int after)
        {
            var prices = await _priceRepository.ListInstrumentPricesAsync(instrumentId);

            var multiplier = (decimal)before / after;
            foreach (var price in prices.Where(p => p.Time <= time))
            {
                price.ChangePrice(price.Price * multiplier);
                price.IncreaseVersion();
                _priceRepository.Update(price);
            }
        }

        private async Task AdjustPreSplitTransactions(int instrumentId, DateTime time, int before, int after)
        {
            var positions = await _positionRepository.ListAllInstrumentPositionsAsync(instrumentId);

            var priceMultiplier = (decimal)before / after;
            var amountMultiplier = 1m / priceMultiplier;
            foreach (var position in positions)
            {
                foreach (var transaction in position.Transactions.Where(t => t.Time <= time))
                {
                    transaction.SetAmount(transaction.Amount * amountMultiplier);
                    transaction.SetPrice(transaction.Price * priceMultiplier);
                }

                position.IncreaseVersion();
                _positionRepository.Update(position);
            }
        }
    }
}
