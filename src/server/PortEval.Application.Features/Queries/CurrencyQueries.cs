using Dapper;
using PortEval.Application.Features.Interfaces.Queries;
using PortEval.Application.Features.Queries.DataQueries;
using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Queries
{
    /// <inheritdoc cref="ICurrencyQueries"/>
    public class CurrencyQueries : ICurrencyQueries
    {
        private readonly IDbConnectionCreator _connectionCreator;

        public CurrencyQueries(IDbConnectionCreator connection)
        {
            _connectionCreator = connection;
        }

        /// <inheritdoc cref="ICurrencyQueries.GetAllCurrencies"/>
        public async Task<QueryResponse<IEnumerable<CurrencyDto>>> GetAllCurrencies()
        {
            var query = CurrencyDataQueries.GetAllCurrenciesQuery();

            using var connection = _connectionCreator.CreateConnection();
            var currencies = await connection.QueryAsync<CurrencyDto>(query.Query, query.Params);

            return new QueryResponse<IEnumerable<CurrencyDto>>
            {
                Status = QueryStatus.Ok,
                Response = currencies
            };
        }

        /// <inheritdoc cref="ICurrencyQueries.GetCurrency"/>
        public async Task<QueryResponse<CurrencyDto>> GetCurrency(string currencyCode)
        {
            var query = CurrencyDataQueries.GetCurrencyQuery(currencyCode);

            using var connection = _connectionCreator.CreateConnection();
            var currency = await connection.QueryFirstOrDefaultAsync<CurrencyDto>(query.Query, query.Params);

            return new QueryResponse<CurrencyDto>
            {
                Status = currency != null ? QueryStatus.Ok : QueryStatus.NotFound,
                Response = currency
            };
        }
    }
}
