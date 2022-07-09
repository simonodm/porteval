using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using System;
using System.Collections.Generic;

namespace PortEval.Application.Services.Queries.DataQueries
{
    internal static class TransactionDataQueries
    {
        public static QueryWrapper<IEnumerable<TransactionDto>> GetTransactions(TransactionFilters filters, DateTime from,
            DateTime to)
        {
            var baseQuery = @"SELECT Transactions.Id, PositionId, Positions.PortfolioId, Time, Amount, Price, Transactions.Note, Instruments.Id,
                              Instruments.Name, Instruments.Symbol, Instruments.Exchange, Instruments.Type, Instruments.CurrencyCode, Instruments.Note,
                              Instruments.IsTracked, Instruments.TrackingInfo_LastUpdate as LastPriceUpdate FROM dbo.Transactions
                              INNER JOIN dbo.Positions ON Transactions.PositionId = Positions.Id
                              INNER JOIN dbo.Instruments ON Positions.InstrumentId = Instruments.Id
                              WHERE Time >= @TimeFrom
                              AND Time <= @TimeTo";
            if (filters != null)
            {
                if (filters.PortfolioId != null)
                {
                    baseQuery += " AND Positions.PortfolioId = @PortfolioId";
                }

                if (filters.PositionId != null)
                {
                    baseQuery += " AND Transactions.PositionId = @PositionId";
                }

                if (filters.InstrumentId != null)
                {
                    baseQuery += " AND Positions.InstrumentId = @InstrumentId";
                }
            }


            baseQuery += " ORDER BY Time";
            return new QueryWrapper<IEnumerable<TransactionDto>>
            {
                Query = baseQuery,
                Params = new
                {
                    TimeFrom = from,
                    TimeTo = to,
                    filters?.PortfolioId,
                    filters?.PositionId,
                    filters?.InstrumentId
                }
            };
        }

        public static QueryWrapper<TransactionDto> GetTransaction(int transactionId)
        {
            return new QueryWrapper<TransactionDto>
            {
                Query =
                    @"SELECT Transactions.Id, PositionId, Positions.PortfolioId, Time, Amount, Price, Transactions.Note, Instruments.Id,
                      Instruments.Name, Instruments.Symbol, Instruments.Exchange, Instruments.Type, Instruments.CurrencyCode, Instruments.Note,
                      Instruments.IsTracked, Instruments.TrackingInfo_LastUpdate as LastPriceUpdate FROM dbo.Transactions
                      INNER JOIN dbo.Positions ON Transactions.PositionId = Positions.Id
                      INNER JOIN dbo.Instruments ON Positions.InstrumentId = Instruments.Id
                      WHERE Transactions.Id = @TransactionId",
                Params = new { TransactionId = transactionId }
            };
        }
    }
}
