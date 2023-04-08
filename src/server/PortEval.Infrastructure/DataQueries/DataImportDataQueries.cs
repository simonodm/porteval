using Dapper;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.DataQueries;

namespace PortEval.Infrastructure.DataQueries
{
    public class DataImportDataQueries : IDataImportDataQueries
    {
        private readonly IDbConnectionCreator _connectionCreator;

        public DataImportDataQueries(IDbConnectionCreator connectionCreator)
        {
            _connectionCreator = connectionCreator;
        }

        public async Task<IEnumerable<CsvTemplateImportDto>> GetAllImportsAsync()
        {
            using var connection = _connectionCreator.CreateConnection();
            var query =
                @"SELECT Id AS ImportId, ErrorLogAvailable, TemplateType, Status, StatusDetails, Time FROM dbo.Imports
                    ORDER BY Time DESC";

            return await connection.QueryAsync<CsvTemplateImportDto>(query);
        }

        public async Task<CsvTemplateImportDto> GetImportAsync(Guid id)
        {
            using var connection = _connectionCreator.CreateConnection();
            var query =
                @"SELECT Id AS ImportId, ErrorLogAvailable, TemplateType, Status, StatusDetails, Time FROM dbo.Imports
                    WHERE Id = @ImportId";

            return await connection.QueryFirstOrDefaultAsync<CsvTemplateImportDto>(query, new { ImportId = id });
        }
    }
}
