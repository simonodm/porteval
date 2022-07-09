﻿using PortEval.Application.Models.DTOs;
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
                          INNER JOIN dbo.Instruments
                          ON Positions.InstrumentId = Instruments.Id"
            };
        }

        public static QueryWrapper<IEnumerable<PositionDto>> GetPortfolioPositions(int portfolioId)
        {
            return new QueryWrapper<IEnumerable<PositionDto>>
            {
                Query = @"SELECT *, TrackingInfo_LastUpdate as LastPriceUpdate FROM dbo.Positions 
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
                Query = @"SELECT *, TrackingInfo_LastUpdate as LastPriceUpdate FROM dbo.Positions
                          INNER JOIN dbo.Instruments
                          ON Positions.InstrumentId = Instruments.Id
                          WHERE Positions.Id = @PositionId",
                Params = new { PositionId = positionId }
            };
        }

        public static QueryWrapper<IEnumerable<TransactionDetailsQueryModel>> GetDetailedTransactionsQuery(
            int positionId, DateTime from, DateTime to)
        {
            return new QueryWrapper<IEnumerable<TransactionDetailsQueryModel>>
            {
                Query = @"WITH rownum_prices_start AS (
	                          SELECT InstrumentId, Price AS InstrumentPriceAtRangeStart,
                                ROW_NUMBER() OVER(PARTITION BY InstrumentId ORDER BY Time DESC) as rownum_start FROM dbo.InstrumentPrices
	                          WHERE Time <= @TimeFrom
                          ), rownum_prices_end AS (
	                          SELECT InstrumentId, Price AS InstrumentPriceAtRangeEnd,
                                ROW_NUMBER() OVER(PARTITION BY InstrumentId ORDER BY Time DESC) as rownum_end FROM dbo.InstrumentPrices
	                          WHERE Time <= @TimeTo
                          )
 
                          SELECT Time, Amount, Price, InstrumentPriceAtRangeStart, InstrumentPriceAtRangeEnd FROM dbo.Positions
                          INNER JOIN (
	                          SELECT PositionId, Time, Amount, Price FROM dbo.Transactions
	                          WHERE Time <= @TimeTo
                          ) AS Transactions ON PositionId = Positions.Id
                          INNER JOIN dbo.Instruments ON Instruments.Id = InstrumentId
                          INNER JOIN (
	                          SELECT rownum_prices_start.InstrumentId, InstrumentPriceAtRangeStart, InstrumentPriceAtRangeEnd FROM rownum_prices_start
	                          INNER JOIN (
		                          SELECT InstrumentId, InstrumentPriceAtRangeEnd FROM rownum_prices_end
		                          WHERE rownum_end = 1
	                          ) AS prices_end ON prices_end.InstrumentId = rownum_prices_start.InstrumentId
	                          WHERE rownum_start = 1
                          ) AS Prices ON Positions.InstrumentId = Prices.InstrumentId
                          WHERE Transactions.PositionId = @PositionId
                          ORDER BY Time DESC",
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
