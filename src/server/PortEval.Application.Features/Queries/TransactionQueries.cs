using Dapper;
using PortEval.Application.Features.Interfaces.Queries;
using PortEval.Application.Features.Queries.DataQueries;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Queries
{
    /// <inheritdoc cref="ITransactionQueries"/>
    public class TransactionQueries : ITransactionQueries
    {
        private readonly IDbConnectionCreator _connectionCreator;

        public TransactionQueries(IDbConnectionCreator connection)
        {
            _connectionCreator = connection;
        }

        /// <inheritdoc cref="ITransactionQueries.GetTransactions"/>
        public async Task<QueryResponse<IEnumerable<TransactionDto>>> GetTransactions(TransactionFilters filters, DateRangeParams dateRange)
        {
            var query = TransactionDataQueries.GetTransactions(filters, dateRange.From,
                dateRange.To);

            using var connection = _connectionCreator.CreateConnection();
            var transactions = await connection.QueryAsync<TransactionDto, InstrumentDto, TransactionDto>(query.Query, (t, i) =>
            {
                t.Instrument = i;
                return t;
            }, query.Params);

            return new QueryResponse<IEnumerable<TransactionDto>>
            {
                Status = QueryStatus.Ok,
                Response = transactions
            };
        }

        /// <inheritdoc cref="ITransactionQueries.GetTransaction"/>
        public async Task<QueryResponse<TransactionDto>> GetTransaction(int transactionId)
        {
            var query = TransactionDataQueries.GetTransaction(transactionId);

            using var connection = _connectionCreator.CreateConnection();
            var queryResponse = await connection.QueryAsync<TransactionDto, InstrumentDto, TransactionDto>(query.Query,
                (t, i) =>
                {
                    t.Instrument = i;
                    return t;
                }, query.Params);

            var transaction = queryResponse?.FirstOrDefault();

            return new QueryResponse<TransactionDto>
            {
                Status = transaction != null ? QueryStatus.Ok : QueryStatus.NotFound,
                Response = transaction
            };
        }
    }
}