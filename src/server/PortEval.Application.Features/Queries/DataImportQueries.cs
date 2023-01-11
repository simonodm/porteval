using Dapper;
using PortEval.Application.Features.Interfaces.Queries;
using PortEval.Application.Features.Queries.DataQueries;
using PortEval.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Queries
{
    public class DataImportQueries : IDataImportQueries
    {
        private readonly IDbConnectionCreator _connectionCreator;

        public DataImportQueries(IDbConnectionCreator connectionCreator)
        {
            _connectionCreator = connectionCreator;
        }

        public async Task<QueryResponse<IEnumerable<CsvTemplateImportDto>>> GetAllImports()
        {
            var query = DataImportDataQueries.GetAllImports();

            using var connection = _connectionCreator.CreateConnection();
            var result = await connection.QueryAsync<CsvTemplateImportDto>(query.Query, query.Params);

            return new QueryResponse<IEnumerable<CsvTemplateImportDto>>
            {
                Status = QueryStatus.Ok,
                Response = result.Select(AssignErrorLogUrl)
            };
        }

        public async Task<QueryResponse<CsvTemplateImportDto>> GetImport(Guid id)
        {
            var query = DataImportDataQueries.GetImport(id);

            using var connection = _connectionCreator.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<CsvTemplateImportDto>(query.Query, query.Params);

            return new QueryResponse<CsvTemplateImportDto>
            {
                Status = result == null ? QueryStatus.NotFound : QueryStatus.Ok,
                Response = result
            };
        }

        private CsvTemplateImportDto AssignErrorLogUrl(CsvTemplateImportDto importDto)
        {
            if (importDto.ErrorLogAvailable)
            {
                importDto.ErrorLogUrl = $"/api/imports/{importDto.ImportId}/log";
            }

            return importDto;
        }
    }
}
