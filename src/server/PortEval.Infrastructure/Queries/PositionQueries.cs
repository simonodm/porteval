using Dapper;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Infrastructure.Queries
{
    public class PositionQueries : IPositionQueries
    {
        private readonly PortEvalDbConnectionCreator _connectionCreator;

        public PositionQueries(PortEvalDbConnectionCreator connectionCreator)
        {
            _connectionCreator = connectionCreator;
        }

        public async Task<IEnumerable<PositionDto>> GetAllPositionsAsync()
        {
            using var connection = _connectionCreator.CreateConnection();
            var query = @"SELECT *, TrackingInfo_LastUpdate as LastPriceUpdate FROM dbo.Positions
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
                          ORDER BY dbo.Instruments.Symbol";

            return await connection.QueryAsync<PositionDto, InstrumentDto, PositionDto>(query, (p, i) =>
            {
                p.Instrument = i;
                return p;
            });
        }

        public async Task<IEnumerable<PositionDto>> GetPortfolioPositionsAsync(int portfolioId)
        {
            using var connection = _connectionCreator.CreateConnection();
            var query = @"SELECT *, TrackingInfo_LastUpdate as LastPriceUpdate FROM dbo.Positions 
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
                          ORDER BY dbo.Instruments.Symbol";

            return await connection.QueryAsync<PositionDto, InstrumentDto, PositionDto>(query, (p, i) =>
            {
                p.Instrument = i;
                return p;
            }, new { PortfolioId = portfolioId });
        }

        public async Task<PositionDto> GetPositionAsync(int positionId)
        {
            using var connection = _connectionCreator.CreateConnection();
            var query = @"SELECT *, TrackingInfo_LastUpdate as LastPriceUpdate FROM dbo.Positions
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
                          AND Positions.Id = @PositionId";

            var result = await connection.QueryAsync<PositionDto, InstrumentDto, PositionDto>(query, (p, i) =>
            {
                p.Instrument = i;
                return p;
            }, new { PositionId = positionId });

            return result.FirstOrDefault();
        }
    }
}
