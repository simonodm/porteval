using System;
using System.Collections.Generic;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Queries.DataQueries
{
    internal static class DataImportDataQueries
    {
        public static QueryWrapper<IEnumerable<CsvTemplateImportDto>> GetAllImports()
        {
            return new QueryWrapper<IEnumerable<CsvTemplateImportDto>>
            {
                Query = @"SELECT Id AS ImportId, ErrorLogAvailable, TemplateType, Status, StatusDetails, Time FROM dbo.Imports
                          ORDER BY Time DESC"
            };
        }

        public static QueryWrapper<CsvTemplateImportDto> GetImport(Guid id)
        {
            return new QueryWrapper<CsvTemplateImportDto>
            {
                Query = @"SELECT Id AS ImportId, ErrorLogAvailable, TemplateType, Status, StatusDetails, Time FROM dbo.Imports
                          WHERE Id = @ImportId",
                Params = new { ImportId = id }
            };
        }
    }
}
