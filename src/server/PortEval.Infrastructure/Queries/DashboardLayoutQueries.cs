using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Models.DTOs;

namespace PortEval.Infrastructure.Queries;

/// <inheritdoc cref="IDashboardLayoutQueries" />
public class DashboardLayoutQueries : IDashboardLayoutQueries
{
    private readonly PortEvalDbConnectionCreator _connectionCreator;

    public DashboardLayoutQueries(PortEvalDbConnectionCreator connectionCreator)
    {
        _connectionCreator = connectionCreator;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DashboardItemDto>> GetDashboardItemsAsync()
    {
        using var connection = _connectionCreator.CreateConnection();
        var query = @"SELECT * FROM dbo.DashboardItems";

        return await connection.QueryAsync<DashboardItemDto>(query);
    }
}