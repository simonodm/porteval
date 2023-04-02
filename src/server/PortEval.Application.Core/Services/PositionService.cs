using System;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Core.Services
{
    /// <inheritdoc cref="IPositionService"/>
    public class PositionService : IPositionService
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly IInstrumentRepository _instrumentRepository;

        public PositionService(IPortfolioRepository portfolioRepository,
            IPositionRepository positionRepository, IInstrumentRepository instrumentRepository)
        {
            _instrumentRepository = instrumentRepository;
            _positionRepository = positionRepository;
            _portfolioRepository = portfolioRepository;
        }

        /// <inheritdoc cref="IPositionService.OpenPositionAsync"/>
        public async Task<Position> OpenPositionAsync(PositionDto options)
        {
            if (options.Amount == null || options.Price == null || options.Time == null)
            {
                throw new OperationNotAllowedException(
                    "An initial transaction is required when opening a new position.");
            }

            var initialTransactionTime = (DateTime)options.Time;
            var initialTransactionPrice = (decimal)options.Price;
            var initialTransactionAmount = (decimal)options.Amount;

            var portfolio = await FetchPortfolio(options.PortfolioId);
            var instrument = await FetchInstrument(options.InstrumentId);
            var createdPosition = Position.Create(portfolio, instrument, options.Note);
            createdPosition.AddTransaction(initialTransactionAmount, initialTransactionPrice,
                initialTransactionTime);
            _positionRepository.Add(createdPosition);
            await _positionRepository.UnitOfWork.CommitAsync();

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

        private async Task<Portfolio> FetchPortfolio(int portfolioId)
        {
            var portfolio = await _portfolioRepository.FindAsync(portfolioId);
            if (portfolio == null)
            {
                throw new ItemNotFoundException($"Portfolio {portfolioId} does not exist.");
            }

            return portfolio;
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
