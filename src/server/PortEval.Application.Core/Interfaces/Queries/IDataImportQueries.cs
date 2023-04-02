using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Queries;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Interfaces.Queries
{
    /// <summary>
    /// Implements high performance read-only bulk import entry queries.
    /// </summary>
    public interface IDataImportQueries
    {
        /// <summary>
        /// Retrieves all data import entries.
        /// </summary>
        /// <returns>A task representing the asynchronous database query operation.</returns>
        Task<QueryResponse<IEnumerable<CsvTemplateImportDto>>> GetAllImports();

        /// <summary>
        /// Retrieves a data import entry by ID.
        /// </summary>
        /// <param name="id">ID of the data import entry.</param>
        /// <returns>A task representing the asynchronous database query operation.</returns>
        Task<QueryResponse<CsvTemplateImportDto>> GetImport(Guid id);
    }
}