using Dapper;
using PortEval.Application.Features.Interfaces.Queries;
using PortEval.Application.Features.Queries.DataQueries;
using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Queries
{
    public class ExchangeQueries : IExchangeQueries
    {
        private readonly IDbConnectionCreator _connectionCreator;

        public ExchangeQueries(IDbConnectionCreator connectionCreator)
        {
            _connectionCreator = connectionCreator;
        }

        public async Task<QueryResponse<IEnumerable<ExchangeDto>>> GetKnownExchanges()
        {
            var query = ExchangeDataQueries.GetKnownExchangesQuery();
            using var connection = _connectionCreator.CreateConnection();
            var result = await connection.QueryAsync<ExchangeDto>(query.Query, query.Params);

            return new QueryResponse<IEnumerable<ExchangeDto>>
            {
                Status = QueryStatus.Ok,
                Response = result
            };
        }
    }
}
