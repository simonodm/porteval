using Dapper;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.DataQueries;

namespace PortEval.Infrastructure.DataQueries
{
    public class ExchangeDataQueries : IExchangeDataQueries
    {
        private readonly IDbConnectionCreator _connectionCreator;

        public ExchangeDataQueries(IDbConnectionCreator connectionCreator)
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
