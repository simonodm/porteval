using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using PortEval.Application.Core.Interfaces.Queries;

namespace PortEval.Infrastructure
{
    /// <inheritdoc cref="IDbConnectionCreator" />
    public class PortEvalDbConnectionCreator : IDbConnectionCreator
    {
        private readonly string _connectionString;
        private IDbConnection _connection;

        public PortEvalDbConnectionCreator(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PortEvalDb");
        }

        /// <inheritdoc />
        public IDbConnection CreateConnection() => _connection ??= new SqlConnection(_connectionString);
    }
}
