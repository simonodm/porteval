using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;

namespace PortEval.Application.Core.Services;

/// <inheritdoc cref="ITransactionService" />
public class TransactionService : ITransactionService
{
    private readonly IInstrumentRepository _instrumentRepository;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly ITransactionQueries _transactionDataQueries;

    /// <summary>
    ///     Initializes the service.
    /// </summary>
    public TransactionService(IPortfolioRepository portfolioRepository, IPositionRepository positionRepository,
        IInstrumentRepository instrumentRepository, ITransactionQueries transactionDataQueries)
    {
        _portfolioRepository = portfolioRepository;
        _positionRepository = positionRepository;
        _instrumentRepository = instrumentRepository;
        _transactionDataQueries = transactionDataQueries;
    }

    /// <inheritdoc />
    public async Task<OperationResponse<IEnumerable<TransactionDto>>> GetTransactionsAsync(TransactionFilters filters,
        DateRangeParams dateRange)
    {
        if (filters.PortfolioId != null && !await _portfolioRepository.ExistsAsync(filters.PortfolioId.Value))
            return new OperationResponse<IEnumerable<TransactionDto>>
            {
                Status = OperationStatus.Error,
                Message = $"Portfolio {filters.PortfolioId} does not exist."
            };

        if (filters.InstrumentId != null && !await _instrumentRepository.ExistsAsync(filters.InstrumentId.Value))
            return new OperationResponse<IEnumerable<TransactionDto>>
            {
                Status = OperationStatus.Error,
                Message = $"Instrument {filters.InstrumentId} does not exist."
            };

        if (filters.PositionId != null && !await _positionRepository.ExistsAsync(filters.PositionId.Value))
            return new OperationResponse<IEnumerable<TransactionDto>>
            {
                Status = OperationStatus.Error,
                Message = $"Position {filters.PositionId} does not exist."
            };

        var transactions =
            await _transactionDataQueries.GetTransactionsAsync(filters, dateRange.From, dateRange.To);

        return new OperationResponse<IEnumerable<TransactionDto>>
        {
            Response = transactions
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<TransactionDto>> GetTransactionAsync(int transactionId)
    {
        var transaction = await _transactionDataQueries.GetTransactionAsync(transactionId);

        return new OperationResponse<TransactionDto>
        {
            Status = transaction != null ? OperationStatus.Ok : OperationStatus.NotFound,
            Message = transaction != null ? "" : $"Transaction {transactionId} does not exist.",
            Response = transaction
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<TransactionDto>> AddTransactionAsync(TransactionDto options)
    {
        var position = await _positionRepository.FindAsync(options.PositionId);
        if (position == null)
            return new OperationResponse<TransactionDto>
            {
                Status = OperationStatus.Error,
                Message = $"Position {options.PositionId} does not exist."
            };
        var createdTransaction = position.AddTransaction(options.Amount, options.Price, options.Time, options.Note);
        position.IncreaseVersion();

        _positionRepository.Update(position);
        await _positionRepository.UnitOfWork.CommitAsync();

        return await GetTransactionAsync(createdTransaction.Id);
    }

    /// <inheritdoc />
    public async Task<OperationResponse<TransactionDto>> UpdateTransactionAsync(TransactionDto options)
    {
        var position = await _positionRepository.FindAsync(options.PositionId);
        if (position == null)
            return new OperationResponse<TransactionDto>
            {
                Status = OperationStatus.Error,
                Message = $"Position {options.PositionId} does not exist."
            };

        var transaction =
            position.UpdateTransaction(options.Id, options.Amount, options.Price, options.Time, options.Note);
        position.IncreaseVersion();
        _positionRepository.Update(position);
        await _positionRepository.UnitOfWork.CommitAsync();

        return await GetTransactionAsync(transaction.Id);
    }

    /// <inheritdoc />
    public async Task<OperationResponse> DeleteTransactionAsync(int transactionId)
    {
        var position = await _positionRepository.FindParentPositionAsync(transactionId);
        var transaction = position?.FindTransaction(transactionId);
        if (transaction == null)
            return new OperationResponse
            {
                Status = OperationStatus.Error,
                Message = $"Transaction {transactionId} does not exist."
            };

        position.RemoveTransaction(transactionId);
        position.IncreaseVersion();
        _positionRepository.Update(position);
        await _positionRepository.UnitOfWork.CommitAsync();

        return new OperationResponse();
    }
}