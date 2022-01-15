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

        public static QueryWrapper<IEnumerable<TransactionDto>> GetPortfolioTransactions(int portfolioId)
        {
            return new QueryWrapper<IEnumerable<TransactionDto>>
            {
                Query = @"SELECT Transactions.Id, Positions.PortfolioId, Transactions.PositionId, Amount, Price, Time, Note FROM dbo.Transactions
                          INNER JOIN dbo.Positions ON Positions.Id = Transactions.PositionId
                          WHERE PortfolioId = @PortfolioId
                          ORDER BY Time DESC",
                Params = new { PortfolioId = portfolioId }
            };
        }

        public static QueryWrapper<IEnumerable<TransactionDetailsQueryModel>> GetPortfolioDetailedTransactions(
            int portfolioId, DateTime from, DateTime to)
        {
            return new QueryWrapper<IEnumerable<TransactionDetailsQueryModel>>
            {
                Query = @"SELECT PortfolioId, pTransactions.PositionId, InstrumentId, pTransactions.Id AS TransactionId, Time, Amount,
	                          Price, Instruments.CurrencyCode AS TransactionCurrency, Portfolios.CurrencyCode AS PortfolioCurrency,
                              posPrices.InstrumentPriceAtRangeStart, posPrices.InstrumentPriceAtRangeEnd, dbo.Portfolios.Note FROM dbo.Portfolios
                          INNER JOIN (
	                          SELECT Transactions.Id, PortfolioId, Time, Amount, Price, PositionId, InstrumentId FROM dbo.Positions
	                          INNER JOIN dbo.Transactions ON PositionId = Positions.Id
                          ) AS pTransactions ON Portfolios.Id = pTransactions.PortfolioId
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
                          ) AS posPrices ON posPrices.PositionId = pTransactions.PositionId
                          WHERE PortfolioId = @PortfolioId
                          AND Time <= @TimeTo",
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