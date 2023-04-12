using Dapper;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Infrastructure.Queries
{
    public class ExchangeQueries : IExchangeQueries
    {
        private readonly PortEvalDbConnectionCreator _connectionCreator;

        public ExchangeQueries(PortEvalDbConnectionCreator connectionCreator)
        {
            _connectionCreator = connectionCreator;
        }

        public async Task<IEnumerable<ExchangeDto>> GetKnownExchangesAsync()
        {
            using var connection = _connectionCreator.CreateConnection();
            var query = @"SELECT * FROM [dbo].[Exchanges] ORDER BY [Symbol]";

            return await connection.QueryAsync<ExchangeDto>(query);
        }
    }
}
