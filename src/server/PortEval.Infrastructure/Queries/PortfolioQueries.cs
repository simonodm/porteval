using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Models.DTOs;

namespace PortEval.Infrastructure.Queries;

/// <inheritdoc cref="IPortfolioQueries" />
public class PortfolioQueries : IPortfolioQueries
{
    private readonly PortEvalDbConnectionCreator _connectionCreator;

    public PortfolioQueries(PortEvalDbConnectionCreator connectionCreator)
    {
        _connectionCreator = connectionCreator;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PortfolioDto>> GetAllPortfoliosAsync()
    {
        using var connection = _connectionCreator.CreateConnection();
        var query = "SELECT * FROM dbo.Portfolios ORDER BY Name";

        return await connection.QueryAsync<PortfolioDto>(query);
    }

    /// <inheritdoc />
    public async Task<PortfolioDto> GetPortfolioAsync(int portfolioId)
    {
        using var connection = _connectionCreator.CreateConnection();
        var query = "SELECT * FROM dbo.Portfolios WHERE Id = @PortfolioId";

        return await connection.QueryFirstOrDefaultAsync<PortfolioDto>(query, new { PortfolioId = portfolioId });
    }
}