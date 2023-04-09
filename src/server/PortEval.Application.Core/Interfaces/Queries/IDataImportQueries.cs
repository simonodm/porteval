using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Interfaces.Queries
{
    public interface IDataImportQueries
    {
        Task<IEnumerable<CsvTemplateImportDto>> GetAllImportsAsync();
        Task<CsvTemplateImportDto> GetImportAsync(Guid id);
    }
}