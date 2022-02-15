using Dapper;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Application.Queries.DataQueries;
using PortEval.Application.Queries.Interfaces;
using PortEval.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Application.Queries
{
    /// <inheritdoc cref="ITransactionQueries"/>
    public class TransactionQueries : ITransactionQueries
    {
        private readonly PortEvalDbConnection _connection;

        public TransactionQueries(PortEvalDbConnection connection)
        {
            _connection = connection;
        }

        /// <inheritdoc cref="ITransactionQueries.GetTransactions"/>
        public async Task<QueryResponse<IEnumerable<TransactionDto>>> GetTransactions(TransactionFilters filters, DateRangeParams dateRange)
        {
            var query = TransactionDataQueries.GetTransactions(filters, dateRange.From,
                dateRange.To);

            using var connection = _connection.CreateConnection();
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

            using var connection = _connection.CreateConnection();
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