using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using PortEval.Application.Core.Interfaces.Queries;

namespace PortEval.Infrastructure
{
    /// <inheritdoc cref="IDbConnectionCreator" />
    public class PortEvalDbConnection : IDbConnectionCreator
    {
        private readonly string _connectionString;

        public PortEvalDbConnection(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PortEvalDb");
        }

        /// <inheritdoc />
        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
