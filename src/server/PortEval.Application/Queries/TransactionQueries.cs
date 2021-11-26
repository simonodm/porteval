using Dapper;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Application.Queries.DataQueries;
using PortEval.Application.Queries.Interfaces;
using PortEval.Infrastructure;
using System.Collections.Generic;
using System.Data;
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

        /// <inheritdoc cref="ITransactionQueries.GetPositionTransactions"/>
        public async Task<QueryResponse<IEnumerable<TransactionDto>>> GetPositionTransactions(int positionId, DateRangeParams dateRange)
        {
            var query = PositionDataQueries.GetPositionTransactions(positionId, dateRange.From,
                dateRange.To);

            using var connection = _connection.CreateConnection();
            if (!(await PositionExists(positionId, connection)))
            {
                return new QueryResponse<IEnumerable<TransactionDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var transactions = await connection.QueryAsync<TransactionDto>(query.Query, query.Params);

            return new QueryResponse<IEnumerable<TransactionDto>>
            {
                Status = QueryStatus.Ok,
                Response = transactions
            };
        }

        /// <inheritdoc cref="ITransactionQueries.GetTransaction"/>
        public async Task<QueryResponse<TransactionDto>> GetTransaction(int positionId, int transactionId)
        {
            var query = PositionDataQueries.GetTransaction(positionId, transactionId);

            using var connection = _connection.CreateConnection();
            if (!(await PositionExists(positionId, connection)))
            {
                return new QueryResponse<TransactionDto>
                {
                    Status = QueryStatus.NotFound
                };
            }
            var transaction = await connection.QueryFirstOrDefaultAsync<TransactionDto>(query.Query, query.Params);

            return new QueryResponse<TransactionDto>
            {
                Status = transaction != null ? QueryStatus.Ok : QueryStatus.NotFound,
                Response = transaction
            };
        }

        private async Task<bool> PositionExists(int positionId, IDbConnection connection)
        {
            var positionQuery = PositionDataQueries.GetPosition(positionId);
            var position = await connection.QueryFirstOrDefaultAsync<PositionDto>(positionQuery.Query, positionQuery.Params);
            return position != null;
        }
    }
}