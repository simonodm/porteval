using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using PortEval.Application.Services.Queries.Interfaces;

namespace PortEval.Infrastructure
{
    public class PortEvalDbConnection : IDbConnectionCreator
    {
        private readonly string _connectionString;

        public PortEvalDbConnection(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PortEvalDb");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
