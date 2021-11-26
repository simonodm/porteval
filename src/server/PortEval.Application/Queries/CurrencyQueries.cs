using Dapper;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Queries.DataQueries;
using PortEval.Application.Queries.Interfaces;
using PortEval.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Queries
{
    /// <inheritdoc cref="ICurrencyQueries"/>
    public class CurrencyQueries : ICurrencyQueries
    {
        private readonly PortEvalDbConnection _connection;

        public CurrencyQueries(PortEvalDbConnection connection)
        {
            _connection = connection;
        }

        /// <inheritdoc cref="ICurrencyQueries.GetAllCurrencies"/>
        public async Task<QueryResponse<IEnumerable<CurrencyDto>>> GetAllCurrencies()
        {
            var query = CurrencyDataQueries.GetAllCurrenciesQuery();

            using var connection = _connection.CreateConnection();
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

            using var connection = _connection.CreateConnection();
            var currency = await connection.QueryFirstOrDefaultAsync<CurrencyDto>(query.Query, query.Params);

            return new QueryResponse<CurrencyDto>
            {
                Status = currency != null ? QueryStatus.Ok : QueryStatus.NotFound,
                Response = currency
            };
        }
    }
}
