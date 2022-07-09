using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Services.Queries.Interfaces
{
    public interface IDataImportQueries
    {
        Task<QueryResponse<IEnumerable<CsvTemplateImportDto>>> GetAllImports();
        Task<QueryResponse<CsvTemplateImportDto>> GetImport(Guid id);
    }
}