using PortEval.Application.Models.DTOs;
using PortEval.Application.Queries.Models;
using System;
using System.Collections.Generic;

namespace PortEval.Application.Queries.DataQueries
{
    internal static class PositionDataQueries
    {
        public static QueryWrapper<IEnumerable<PositionDto>> GetPortfolioPositionsWithInstruments(int portfolioId)
        {
            return new QueryWrapper<IEnumerable<PositionDto>>
            {
                Query = @"SELECT * FROM dbo.Positions 
                          INNER JOIN dbo.Instruments
                          ON Positions.InstrumentId = Instruments.Id
                          WHERE Positions.PortfolioId = @PortfolioId",
                Params = new { PortfolioId = portfolioId }
            };
        }

        public static QueryWrapper<PositionDto> GetPosition(int positionId)
        {
            return new QueryWrapper<PositionDto>
            {
                Query = @"SELECT * FROM dbo.Positions
                          INNER JOIN dbo.Instruments
                          ON Positions.InstrumentId = Instruments.Id
                          WHERE Positions.Id = @PositionId",
                Params = new { PositionId = positionId }
            };
        }

        public static QueryWrapper<IEnumerable<TransactionDto>> GetPositionTransactions(int positionId, DateTime from, DateTime to)
        {
            return new QueryWrapper<IEnumerable<TransactionDto>>
            {
                Query = @"SELECT Transactions.Id, PositionId, Positions.PortfolioId, Time, Amount, Price, Transactions.Note FROM dbo.Transactions
                          INNER JOIN dbo.Positions ON Transactions.PositionId = Positions.Id
                          WHERE PositionId = @PositionId
                          AND Time >= @TimeFrom
                          AND Time <= @TimeTo",
                Params = new
                {
                    PositionId = positionId,
                    TimeFrom = from,
                    TimeTo = to
                }
            };
        }

        public static QueryWrapper<TransactionDto> GetTransaction(int positionId, int transactionId)
        {
            return new QueryWrapper<TransactionDto>
            {
                Query =
                    @"SELECT Transactions.Id, PositionId, Positions.PortfolioId, Time, Amount, Price, Note FROM dbo.Transactions
                          INNER JOIN dbo.Positions ON Transactions.PositionId = Positions.Id
                          WHERE PositionId = @PositionId
                          AND Transactions.Id = @TransactionId",
                Params = new { PositionId = positionId, TransactionId = transactionId }
            };
        }

        public static QueryWrapper<IEnumerable<TransactionDetailsQueryModel>> GetDetailedTransactionsQuery(
            int positionId, DateTime from, DateTime to)
        {
            return new QueryWrapper<IEnumerable<TransactionDetailsQueryModel>>
            {
                Query = @"SELECT Time, Amount, Price, posPrices.InstrumentPriceAtRangeStart, posPrices.InstrumentPriceAtRangeEnd FROM dbo.Positions
                          INNER JOIN dbo.Transactions ON PositionId = Positions.Id
                          INNER JOIN dbo.Instruments ON Instruments.Id = InstrumentId
                          INNER JOIN (
	                          SELECT Positions.Id AS PositionId, ip2.Price AS InstrumentPriceAtRangeStart, ip1.Price AS InstrumentPriceAtRangeEnd FROM dbo.Positions
	                          INNER JOIN (
		                          SELECT InstrumentPrices.InstrumentId, MAX(Time) AS PriceTimeRangeEnd, PriceTimeRangeStart FROM dbo.InstrumentPrices
		                          INNER JOIN (
			                          SELECT InstrumentId, MAX(Time) AS PriceTimeRangeStart FROM dbo.InstrumentPrices 
			                          WHERE Time <= @TimeFrom 
			                          GROUP BY InstrumentId
		                          ) AS pricesFrom ON InstrumentPrices.InstrumentId = pricesFrom.InstrumentId
		                          WHERE Time <= @TimeTo
		                          GROUP BY InstrumentPrices.InstrumentId, PriceTimeRangeStart
	                          ) AS prices ON prices.InstrumentId = Positions.InstrumentId
	                          INNER JOIN dbo.InstrumentPrices AS ip1 ON ip1.InstrumentId = prices.InstrumentId AND ip1.Time = prices.PriceTimeRangeEnd
	                          INNER JOIN dbo.InstrumentPrices AS ip2 ON ip2.InstrumentId = prices.InstrumentId AND ip2.Time = prices.PriceTimeRangeStart
                          ) AS posPrices ON posPrices.PositionId = Transactions.PositionId
                          WHERE Transactions.PositionId = @PositionId
                          AND Time <= @TimeTo",
                Params = new { PositionId = positionId, TimeFrom = from, TimeTo = to }
            };
        }

        public static QueryWrapper<SingleTimeQueryModel> GetPositionFirstTransactionTime(int positionId)
        {
            return new QueryWrapper<SingleTimeQueryModel>
            {
                Query = @"SELECT MIN(Time) AS Time FROM dbo.Transactions 
                          INNER JOIN dbo.Positions ON Positions.Id = Transactions.PositionId
                          WHERE Positions.Id = @PositionId",
                Params = new { PositionId = positionId }
            };
        }
    }
}
