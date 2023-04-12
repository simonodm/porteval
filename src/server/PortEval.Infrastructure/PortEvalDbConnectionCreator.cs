using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace PortEval.Infrastructure
{
    /// <summary>
    /// A factory for creating a connection to the PortEval database.
    /// </summary>
    public class PortEvalDbConnectionCreator
    {
        private readonly string _connectionString;

        public PortEvalDbConnectionCreator(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PortEvalDb");
        }

        /// <summary>
        /// Creates a connection to the PortEval database.
        /// </summary>
        /// <returns>An <see cref="IDbConnection"/> instance connected to the PortEval database.</returns>
        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
