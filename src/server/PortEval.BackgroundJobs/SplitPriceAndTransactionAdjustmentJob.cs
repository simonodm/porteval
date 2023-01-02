﻿using Microsoft.Extensions.Logging;
using PortEval.Application.Features.Interfaces.BackgroundJobs;
using PortEval.Application.Features.Interfaces.Repositories;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Models.DTOs.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Domain.Models.Entities;

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
                await ProcessSplit(split);
            }

            foreach (var split in rolledBackSplits)
            {
                await ProcessSplitRollback(split);
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

        private async Task ProcessSplit(InstrumentSplit split)
        {
            var prices = await _priceRepository.ListInstrumentPricesAsync(split.InstrumentId);
            var positions = await _positionRepository.ListAllInstrumentPositionsAsync(split.InstrumentId);

            foreach (var price in prices)
            {
                price.AdjustForSplit(split);
                _priceRepository.Update(price);
            }

            foreach (var position in positions)
            {
                foreach (var transaction in position.Transactions)
                {
                    transaction.AdjustForSplit(split);
                }

                _positionRepository.Update(position);
            }

            split.MarkAsProcessed();
            split.IncreaseVersion();
            _splitRepository.Update(split);
        }

        private async Task ProcessSplitRollback(InstrumentSplit split)
        {
            var prices = await _priceRepository.ListInstrumentPricesAsync(split.InstrumentId);
            var positions = await _positionRepository.ListAllInstrumentPositionsAsync(split.InstrumentId);

            foreach (var price in prices)
            {
                price.AdjustForSplitRollback(split);
                _priceRepository.Update(price);
            }

            foreach (var position in positions)
            {
                foreach (var transaction in position.Transactions)
                {
                    transaction.AdjustForSplitRollback(split);
                }

                _positionRepository.Update(position);
            }

            split.MarkAsRolledBack();
            split.IncreaseVersion();
            _splitRepository.Update(split);
        }
    }
}