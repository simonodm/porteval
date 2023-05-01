using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;

namespace PortEval.Infrastructure.Queries;

/// <inheritdoc cref="ITransactionQueries" />
public class TransactionQueries : ITransactionQueries
{
    private readonly PortEvalDbConnectionCreator _connectionCreator;

    public TransactionQueries(PortEvalDbConnectionCreator connectionCreator)
    {
        _connectionCreator = connectionCreator;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TransactionDto>> GetTransactionsAsync(TransactionFilters filters, DateTime from,
        DateTime to)
    {
        using var connection = _connectionCreator.CreateConnection();
        var query =
            @"SELECT Transactions.Id, PositionId, Positions.PortfolioId, Time, Amount, Price, Transactions.Note, Instruments.Id,
                      Instruments.Name, Instruments.Symbol, Instruments.Exchange, Instruments.Type, Instruments.CurrencyCode, Instruments.Note,
                      Instruments.TrackingStatus, Instruments.TrackingInfo_LastUpdate as LastPriceUpdate FROM dbo.Transactions
                      INNER JOIN dbo.Positions ON Transactions.PositionId = Positions.Id
                      INNER JOIN dbo.Instruments ON Positions.InstrumentId = Instruments.Id
                      WHERE Time >= @TimeFrom
                      AND Time <= @TimeTo";
        if (filters != null)
        {
            if (filters.PortfolioId != null)
            {
                query += " AND Positions.PortfolioId = @PortfolioId";
            }

            if (filters.PositionId != null)
            {
                query += " AND Transactions.PositionId = @PositionId";
            }

            if (filters.InstrumentId != null)
            {
                query += " AND Positions.InstrumentId = @InstrumentId";
            }
        }

        query += " ORDER BY Time";

        return await connection.QueryAsync<TransactionDto, InstrumentDto, TransactionDto>(query, (t, i) =>
        {
            t.Instrument = i;
            return t;
        }, new
        {
            TimeFrom = from,
            TimeTo = to,
            filters?.PortfolioId,
            filters?.PositionId,
            filters?.InstrumentId
        });
    }

    /// <inheritdoc />
    public async Task<TransactionDto> GetTransactionAsync(int transactionId)
    {
        using var connection = _connectionCreator.CreateConnection();
        var query =
            @"SELECT Transactions.Id, PositionId, Positions.PortfolioId, Time, Amount, Price, Transactions.Note, Instruments.Id,
                      Instruments.Name, Instruments.Symbol, Instruments.Exchange, Instruments.Type, Instruments.CurrencyCode, Instruments.Note,
                      Instruments.TrackingStatus, Instruments.TrackingInfo_LastUpdate as LastPriceUpdate FROM dbo.Transactions
                      INNER JOIN dbo.Positions ON Transactions.PositionId = Positions.Id
                      INNER JOIN dbo.Instruments ON Positions.InstrumentId = Instruments.Id
                      WHERE Transactions.Id = @TransactionId";

        var result = await connection.QueryAsync<TransactionDto, InstrumentDto, TransactionDto>(query, (t, i) =>
        {
            t.Instrument = i;
            return t;
        }, new { TransactionId = transactionId });
        return result.FirstOrDefault();
    }
}