using System;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.Interfaces;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using System.Threading.Tasks;

namespace PortEval.Application.Services
{
    /// <inheritdoc cref="IPositionService"/>
    public class PositionService : IPositionService
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly IInstrumentPriceRepository _instrumentPriceRepository;

        public PositionService(IPortfolioRepository portfolioRepository,
            IPositionRepository positionRepository, IInstrumentRepository instrumentRepository, IInstrumentPriceRepository instrumentPriceRepository)
        {
            _instrumentRepository = instrumentRepository;
            _instrumentPriceRepository = instrumentPriceRepository;
            _positionRepository = positionRepository;
            _portfolioRepository = portfolioRepository;
        }

        /// <inheritdoc cref="IPositionService.OpenPositionAsync"/>
        public async Task<Position> OpenPositionAsync(PositionDto options)
        {
            await ValidatePortfolioExists(options.PortfolioId);

            if (options.InitialTransaction == default)
            {
                throw new OperationNotAllowedException(
                    "An initial transaction is required when opening a new position.");
            }

            var instrument = await FetchInstrument(options.InstrumentId);
            if (instrument.Type == InstrumentType.Index)
            {
                throw new OperationNotAllowedException("Cannot create a position for an instrument of type index.");
            }

            // create a price point at transaction time if there is no price OR if transaction price is different
            var existingPrice =
                await _instrumentPriceRepository.FindPriceAt(options.InstrumentId, options.InitialTransaction.Time);
            if (existingPrice == null || existingPrice.Price != options.InitialTransaction.Price)
            {
                var newPrice = new InstrumentPrice(options.InitialTransaction.Time, options.InitialTransaction.Price,
                    options.InstrumentId);
                _instrumentPriceRepository.AddInstrumentPrice(newPrice);
            }

            var createdPosition = new Position(options.PortfolioId, options.InstrumentId, options.Note);
            createdPosition.AddTransaction(options.InitialTransaction.Amount, options.InitialTransaction.Price,
                options.InitialTransaction.Time);
            _positionRepository.Add(createdPosition);
            await _positionRepository.UnitOfWork.CommitAsync();
            return createdPosition;
        }

        /// <inheritdoc cref="IPositionService.UpdatePositionAsync"/>
        public async Task<Position> UpdatePositionAsync(PositionDto options)
        {
            var position = await _positionRepository.FindAsync(options.Id);

            position.SetNote(options.Note);
            position.IncreaseVersion();
            _positionRepository.Update(position);
            await _positionRepository.UnitOfWork.CommitAsync();

            return position;
        }

        /// <inheritdoc cref="IPositionService.RemovePositionAsync"/>
        public async Task RemovePositionAsync(int positionId)
        {
            await _positionRepository.Delete(positionId);
            await _positionRepository.UnitOfWork.CommitAsync();
        }

        private async Task ValidatePortfolioExists(int portfolioId)
        {
            if (!(await _portfolioRepository.Exists(portfolioId)))
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
