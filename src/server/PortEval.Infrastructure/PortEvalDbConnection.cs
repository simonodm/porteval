using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace PortEval.Infrastructure
{
    public class PortEvalDbConnection
    {
        private readonly string _connectionString;

        public PortEvalDbConnection(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PortEvalDb");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
