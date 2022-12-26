using PortEval.Application.Features.Interfaces.Repositories;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using System;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Services
{
    /// <inheritdoc cref="IPositionService"/>
    public class PositionService : IPositionService
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly IInstrumentRepository _instrumentRepository;

        private readonly IInstrumentPriceService _instrumentPriceService;

        public PositionService(IPortfolioRepository portfolioRepository,
            IPositionRepository positionRepository, IInstrumentRepository instrumentRepository, IInstrumentPriceService instrumentPriceService)
        {
            _instrumentRepository = instrumentRepository;
            _positionRepository = positionRepository;
            _portfolioRepository = portfolioRepository;
            _instrumentPriceService = instrumentPriceService;
        }

        /// <inheritdoc cref="IPositionService.OpenPositionAsync"/>
        public async Task<Position> OpenPositionAsync(PositionDto options)
        {
            await ValidatePortfolioExists(options.PortfolioId);

            if (options.Amount == null || options.Price == null || options.Time == null)
            {
                throw new OperationNotAllowedException(
                    "An initial transaction is required when opening a new position.");
            }

            var initialTransactionTime = (DateTime)options.Time;
            var initialTransactionPrice = (decimal)options.Price;
            var initialTransactionAmount = (decimal)options.Amount;

            var instrument = await FetchInstrument(options.InstrumentId);
            if (instrument.Type == InstrumentType.Index)
            {
                throw new OperationNotAllowedException("Cannot create a position for an index.");
            }

            var createdPosition = Position.Create(options.PortfolioId, options.InstrumentId, options.Note);
            createdPosition.AddTransaction(initialTransactionAmount, initialTransactionPrice,
                initialTransactionTime);
            _positionRepository.Add(createdPosition);
            await _positionRepository.UnitOfWork.CommitAsync();

            await _instrumentPriceService.AddPriceIfNotExistsAsync(options.InstrumentId, initialTransactionTime, initialTransactionPrice);

            return createdPosition;
        }

        /// <inheritdoc cref="IPositionService.UpdatePositionAsync"/>
        public async Task<Position> UpdatePositionAsync(PositionDto options)
        {
            var position = await _positionRepository.FindAsync(options.Id);
            if (position == null)
            {
                throw new ItemNotFoundException($"Position {options.Id} does not exist.");
            }

            position.SetNote(options.Note);
            position.IncreaseVersion();
            _positionRepository.Update(position);
            await _positionRepository.UnitOfWork.CommitAsync();

            return position;
        }

        /// <inheritdoc cref="IPositionService.RemovePositionAsync"/>
        public async Task RemovePositionAsync(int positionId)
        {
            if (!await _positionRepository.ExistsAsync(positionId))
            {
                throw new ItemNotFoundException($"Position {positionId} does not exist.");
            }
            await _positionRepository.DeleteAsync(positionId);
            await _positionRepository.UnitOfWork.CommitAsync();
        }

        private async Task ValidatePortfolioExists(int portfolioId)
        {
            if (!await _portfolioRepository.ExistsAsync(portfolioId))
            {
                throw new ItemNotFoundException($"Portfolio {portfolioId} does not exist.");
            }
        }

        private async Task<Instrument> FetchInstrument(int instrumentId)
        {
            var instrument = await _instrumentRepository.FindAsync(instrumentId);
            if (instrument == null)
            {
                throw new ItemNotFoundException($"Instrument {instrumentId} does not exist.");
            }

            return instrument;
        }
    }
}
