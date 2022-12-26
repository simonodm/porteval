﻿using PortEval.Application.Features.Interfaces.Repositories;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Services
{
    /// <inheritdoc cref="ITransactionService"/>
    public class TransactionService : ITransactionService
    {
        private readonly IPositionRepository _positionRepository;

        private readonly IInstrumentPriceService _instrumentPriceService;

        public TransactionService(IPositionRepository positionRepository, IInstrumentPriceService instrumentPriceService)
        {
            _positionRepository = positionRepository;
            _instrumentPriceService = instrumentPriceService;
        }

        /// <inheritdoc cref="ITransactionService.AddTransactionAsync"/>
        public async Task<Transaction> AddTransactionAsync(TransactionDto options)
        {
            var position = await FindPosition(options.PositionId);
            var createdTransaction = position.AddTransaction(options.Amount, options.Price, options.Time, options.Note);
            position.IncreaseVersion();

            _positionRepository.Update(position);
            await _positionRepository.UnitOfWork.CommitAsync();

            await _instrumentPriceService.AddPriceIfNotExistsAsync(position.InstrumentId, options.Time, options.Price);

            return createdTransaction;
        }

        /// <inheritdoc cref="ITransactionService.UpdateTransactionAsync"/>
        public async Task<Transaction> UpdateTransactionAsync(TransactionDto options)
        {
            var position = await FindPosition(options.PositionId);

            var transaction = position.UpdateTransaction(options.Id, options.Amount, options.Price, options.Time, options.Note);
            position.IncreaseVersion();
            _positionRepository.Update(position);
            await _positionRepository.UnitOfWork.CommitAsync();

            await _instrumentPriceService.AddPriceIfNotExistsAsync(position.InstrumentId, options.Time, options.Price);

            return transaction;
        }

        /// <inheritdoc cref="ITransactionService.DeleteTransactionAsync"/>
        public async Task DeleteTransactionAsync(int transactionId)
        {
            var position = await _positionRepository.FindParentPositionAsync(transactionId);
            if (position == null)
            {
                throw new ItemNotFoundException($"Transaction {transactionId} does not exist.");
            }

            position.RemoveTransaction(transactionId);
            position.IncreaseVersion();
            _positionRepository.Update(position);
            await _positionRepository.UnitOfWork.CommitAsync();
        }

        /// <summary>
        /// Retrieves a position by its ID.
        /// </summary>
        /// <param name="positionId">Position ID</param>
        /// <exception cref="ItemNotFoundException">Thrown if no position was found with the supplied ID.</exception>
        /// <returns>A task representing the asynchronous search operation. The task result contains the found position entity.</returns>
        private async Task<Position> FindPosition(int positionId)
        {
            var position = await _positionRepository.FindAsync(positionId);
            if (position == null)
            {
                throw new ItemNotFoundException($"Position {positionId} does not exist.");
            }

            return position;
        }

        /// <summary>
        /// Retrieves a transaction by its ID and its parent position's ID.
        /// </summary>
        /// <param name="positionId">Parent position ID.</param>
        /// <param name="transactionId">Transaction ID.</param>
        /// <exception cref="ItemNotFoundException">Thrown if no portfolio, position or transaction were found with the supplied IDs.</exception>
        /// <returns>A task representing the asynchronous search operation. The task result contains the found transaction entity.</returns>
        private async Task<Transaction> FindTransaction(int positionId, int transactionId)
        {
            var position = await FindPosition(positionId);
            var transaction = position.FindTransaction(transactionId);
            if (transaction == null)
            {
                throw new ItemNotFoundException($"Transaction {transactionId} not found in position {positionId}.");
            }

            return transaction;
        }
    }
}