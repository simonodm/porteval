using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PortEval.Application.Features.Interfaces.Queries;
using System.Data;

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
