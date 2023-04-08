using Dapper;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.DataQueries;

namespace PortEval.Infrastructure.DataQueries
{
    public class PortfolioDataQueries : IPortfolioDataQueries
    {
        private readonly IDbConnectionCreator _connectionCreator;

        public PortfolioDataQueries(IDbConnectionCreator connectionCreator)
        {
            _connectionCreator = connectionCreator;
        }

        public async Task<IEnumerable<PortfolioDto>> GetAllPortfoliosAsync()
        {
            using var connection = _connectionCreator.CreateConnection();
            var query = "SELECT * FROM dbo.Portfolios ORDER BY Name";

            return await connection.QueryAsync<PortfolioDto>(query);
        }

        public async Task<PortfolioDto> GetPortfolio(int portfolioId)
        {
            using var connection = _connectionCreator.CreateConnection();
            var query = "SELECT * FROM dbo.Portfolios WHERE Id = @PortfolioId";

            return await connection.QueryFirstOrDefaultAsync<PortfolioDto>(query, new { PortfolioId = portfolioId });
        }
    }
}
