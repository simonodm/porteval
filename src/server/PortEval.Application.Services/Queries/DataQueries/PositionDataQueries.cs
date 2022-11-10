using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.Queries.Models;
using System;
using System.Collections.Generic;

namespace PortEval.Application.Services.Queries.DataQueries
{
    internal static class PositionDataQueries
    {
        public static QueryWrapper<IEnumerable<PositionDto>> GetPositions()
        {
            return new QueryWrapper<IEnumerable<PositionDto>>
            {
                Query = @"SELECT *, TrackingInfo_LastUpdate as LastPriceUpdate FROM dbo.Positions
                          LEFT JOIN (
	                          SELECT [PositionId], SUM([Amount]) AS PositionSize FROM [dbo].[Transactions]
	                          GROUP BY [PositionId]
                          ) AS T on T.[PositionId] = dbo.Positions.Id
                          INNER JOIN dbo.Instruments
                          ON Positions.InstrumentId = Instruments.Id
                          LEFT JOIN (SELECT ROW_NUMBER() OVER (PARTITION BY InstrumentId ORDER BY Time DESC) row_num, InstrumentId AS PriceInstrumentId, Price as CurrentPrice FROM dbo.InstrumentPrices) AS p
                          ON p.PriceInstrumentId = dbo.Instruments.Id
                          WHERE (p.row_num = 1
                          OR p.row_num IS NULL)
                          ORDER BY dbo.Instruments.Symbol"
            };
        }

        public static QueryWrapper<IEnumerable<PositionDto>> GetPortfolioPositions(int portfolioId)
        {
            return new QueryWrapper<IEnumerable<PositionDto>>
            {
                Query = @"SELECT *, TrackingInfo_LastUpdate as LastPriceUpdate FROM dbo.Positions 
                          LEFT JOIN (
	                          SELECT [PositionId], SUM([Amount]) AS PositionSize FROM [dbo].[Transactions]
	                          GROUP BY [PositionId]
                          ) AS T on T.[PositionId] = dbo.Positions.Id
                          INNER JOIN dbo.Instruments
                          ON Positions.InstrumentId = Instruments.Id
                          LEFT JOIN (SELECT ROW_NUMBER() OVER (PARTITION BY InstrumentId ORDER BY Time DESC) row_num, InstrumentId AS PriceInstrumentId, Price AS CurrentPrice FROM dbo.InstrumentPrices) AS p
                          ON p.PriceInstrumentId = dbo.Instruments.Id
                          WHERE (p.row_num = 1
                          OR p.row_num IS NULL)
                          AND Positions.PortfolioId = @PortfolioId
                          ORDER BY dbo.Instruments.Symbol",
                Params = new { PortfolioId = portfolioId }
            };
        }

        public static QueryWrapper<PositionDto> GetPosition(int positionId)
        {
            return new QueryWrapper<PositionDto>
            {
                Query = @"SELECT *, TrackingInfo_LastUpdate as LastPriceUpdate FROM dbo.Positions
                          LEFT JOIN (
	                          SELECT [PositionId], SUM([Amount]) AS PositionSize FROM [dbo].[Transactions]
	                          GROUP BY [PositionId]
                          ) AS T on T.[PositionId] = dbo.Positions.Id
                          INNER JOIN dbo.Instruments
                          ON Positions.InstrumentId = Instruments.Id
                          LEFT JOIN (SELECT ROW_NUMBER() OVER (PARTITION BY InstrumentId ORDER BY Time DESC) row_num, InstrumentId AS PriceInstrumentId, Price as CurrentPrice FROM dbo.InstrumentPrices) AS p
                          ON p.PriceInstrumentId = dbo.Instruments.Id
                          WHERE (p.row_num = 1
                          OR p.row_num IS NULL)
                          AND Positions.Id = @PositionId",
                Params = new { PositionId = positionId }
            };
        }

        public static QueryWrapper<IEnumerable<TransactionDetailsQueryModel>> GetDetailedTransactionsQuery(
            int positionId, DateTime transactionFrom, DateTime transactionTo, DateTime priceFrom, DateTime priceTo)
        {
            return new QueryWrapper<IEnumerable<TransactionDetailsQueryModel>>
            {
                Query = @"WITH rownum_prices_start AS (
	                          SELECT InstrumentId, Price AS InstrumentPriceAtRangeStart,
                                ROW_NUMBER() OVER(PARTITION BY InstrumentId ORDER BY Time DESC) as rownum_start FROM dbo.InstrumentPrices
	                          WHERE Time <= @PriceTimeFrom
                          ), rownum_prices_end AS (
	                          SELECT InstrumentId, Price AS InstrumentPriceAtRangeEnd,
                                ROW_NUMBER() OVER(PARTITION BY InstrumentId ORDER BY Time DESC) as rownum_end FROM dbo.InstrumentPrices
	                          WHERE Time <= @PriceTimeTo
                          )
 
                          SELECT Time, Amount, Price, InstrumentPriceAtRangeStart, InstrumentPriceAtRangeEnd FROM dbo.Positions
                          INNER JOIN (
	                          SELECT PositionId, Time, Amount, Price FROM dbo.Transactions
	                          WHERE Time <= @TransactionTimeTo
                              AND Time >= @TransactionTimeFrom
                          ) AS Transactions ON PositionId = Positions.Id
                          INNER JOIN dbo.Instruments ON Instruments.Id = InstrumentId
                          LEFT JOIN (
	                          SELECT rownum_prices_start.InstrumentId, InstrumentPriceAtRangeStart FROM rownum_prices_start
	                          WHERE rownum_start = 1
                          ) AS PricesStart ON PricesStart.InstrumentId = Instruments.Id
                          LEFT JOIN (
	                          SELECT rownum_prices_end.InstrumentId, InstrumentPriceAtRangeEnd FROM rownum_prices_end
	                          WHERE rownum_end = 1
                          ) AS PricesEnd on PricesEnd.InstrumentId = Instruments.Id
                          WHERE Transactions.PositionId = @PositionId
                          ORDER BY Time DESC",
                Params = new
                {
                    PositionId = positionId,
                    TransactionTimeFrom = transactionFrom,
                    TransactionTimeTo = transactionTo,
                    PriceTimeFrom = priceFrom,
                    PriceTimeTo = priceTo
                }
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
