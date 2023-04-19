using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace PortEval.Infrastructure;

/// <summary>
///     A factory for creating a connection to the PortEval database.
/// </summary>
public class PortEvalDbConnectionCreator
{
    private readonly string _connectionString;

    /// <summary>
    ///     Initializes the factory based on the provided configuration object.
    /// </summary>
    /// <param name="configuration">The application's configuration.</param>
    public PortEvalDbConnectionCreator(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("PortEvalDb");
    }

    /// <summary>
    ///     Creates a connection to the PortEval database.
    /// </summary>
    /// <returns>An <see cref="IDbConnection" /> instance connected to the PortEval database.</returns>
    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}