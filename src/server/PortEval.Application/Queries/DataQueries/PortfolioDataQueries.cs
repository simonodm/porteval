using PortEval.Application.Models.DTOs;
using PortEval.Application.Queries.Models;
using System;
using System.Collections.Generic;

namespace PortEval.Application.Queries.DataQueries
{
    internal static class PortfolioDataQueries
    {
        public static QueryWrapper<IEnumerable<PortfolioDto>> GetAllPortfolios()
        {
            return new QueryWrapper<IEnumerable<PortfolioDto>>
            {
                Query = "SELECT * FROM dbo.Portfolios ORDER BY Name"
            };
        }

        public static QueryWrapper<PortfolioDto> GetPortfolio(int portfolioId)
        {
            return new QueryWrapper<PortfolioDto>
            {
                Query = "SELECT * FROM dbo.Portfolios WHERE Id = @PortfolioId",
                Params = new { PortfolioId = portfolioId }
            };
        }

        public static QueryWrapper<IEnumerable<TransactionDetailsQueryModel>> GetPortfolioDetailedTransactions(
            int portfolioId, DateTime from, DateTime to)
        {
            return new QueryWrapper<IEnumerable<TransactionDetailsQueryModel>>
            {
                Query = @"WITH rownum_prices_start AS (
	                          SELECT InstrumentId, Price AS InstrumentPriceAtRangeStart, ROW_NUMBER() OVER(PARTITION BY InstrumentId ORDER BY Time DESC) as rownum_start FROM dbo.InstrumentPrices
	                          WHERE Time <= @TimeFrom
                          ), rownum_prices_end AS (
	                          SELECT InstrumentId, Price AS InstrumentPriceAtRangeEnd, ROW_NUMBER() OVER(PARTITION BY InstrumentId ORDER BY Time DESC) as rownum_end FROM dbo.InstrumentPrices
	                          WHERE Time <= @TimeTo
                          )
 
                          SELECT PortfolioId, Instruments.Id as InstrumentId, pTransactions.PositionId, pTransactions.Id AS TransactionId, Time, Amount,
	                          Price, Instruments.CurrencyCode AS TransactionCurrency, Portfolios.CurrencyCode AS PortfolioCurrency,
                              InstrumentPriceAtRangeStart, InstrumentPriceAtRangeEnd, dbo.Portfolios.Note FROM dbo.Portfolios
                          INNER JOIN (
	                          SELECT Transactions.Id, PortfolioId, Time, Amount, Price, PositionId, InstrumentId FROM dbo.Positions
	                          INNER JOIN dbo.Transactions ON PositionId = Positions.Id
                              WHERE Time <= @TimeTo
                          ) AS pTransactions ON Portfolios.Id = pTransactions.PortfolioId
                          INNER JOIN dbo.Instruments ON Instruments.Id = InstrumentId
                          INNER JOIN (
	                          SELECT rownum_prices_start.InstrumentId, InstrumentPriceAtRangeStart, InstrumentPriceAtRangeEnd FROM rownum_prices_start
	                          INNER JOIN (
		                          SELECT InstrumentId, InstrumentPriceAtRangeEnd FROM rownum_prices_end
		                          WHERE rownum_end = 1
	                          ) AS prices_end ON prices_end.InstrumentId = rownum_prices_start.InstrumentId
	                          WHERE rownum_start = 1
                          ) AS Prices ON Prices.InstrumentId = Instruments.Id
                          WHERE PortfolioId = @PortfolioId",
                Params = new { PortfolioId = portfolioId, TimeFrom = from, TimeTo = to }
            };
        }

        public static QueryWrapper<SingleTimeQueryModel> GetPortfolioFirstTransaction(int portfolioId)
        {
            return new QueryWrapper<SingleTimeQueryModel>
            {
                Query = @"SELECT MIN(Time) AS Time FROM dbo.Transactions 
                          INNER JOIN dbo.Positions ON Positions.Id = Transactions.PositionId
                          WHERE PortfolioId = @PortfolioId",
                Params = new { PortfolioId = portfolioId }
            };
        }

    }
}