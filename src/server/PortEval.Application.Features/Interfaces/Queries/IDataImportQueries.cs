using PortEval.Application.Features.Queries;
using PortEval.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Interfaces.Queries
{
    public interface IDataImportQueries
    {
        Task<QueryResponse<IEnumerable<CsvTemplateImportDto>>> GetAllImports();
        Task<QueryResponse<CsvTemplateImportDto>> GetImport(Guid id);
    }
}