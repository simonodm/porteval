using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Models.DTOs;

namespace PortEval.Infrastructure.Queries;

/// <inheritdoc cref="IDataImportQueries" />
public class DataImportQueries : IDataImportQueries
{
    private readonly PortEvalDbConnectionCreator _connectionCreator;

    public DataImportQueries(PortEvalDbConnectionCreator connectionCreator)
    {
        _connectionCreator = connectionCreator;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CsvTemplateImportDto>> GetAllImportsAsync()
    {
        using var connection = _connectionCreator.CreateConnection();
        var query =
            @"SELECT Id AS ImportId, ErrorLogAvailable, TemplateType, Status, StatusDetails, Time FROM dbo.Imports
                    ORDER BY Time DESC";

        return await connection.QueryAsync<CsvTemplateImportDto>(query);
    }

    /// <inheritdoc />
    public async Task<CsvTemplateImportDto> GetImportAsync(Guid id)
    {
        using var connection = _connectionCreator.CreateConnection();
        var query =
            @"SELECT Id AS ImportId, ErrorLogAvailable, TemplateType, Status, StatusDetails, Time FROM dbo.Imports
                    WHERE Id = @ImportId";

        return await connection.QueryFirstOrDefaultAsync<CsvTemplateImportDto>(query, new { ImportId = id });
    }
}