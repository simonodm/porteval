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
        /// <returns>
        /// A task representing the asynchronous database query operation.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <see cref="IEnumerable{T}"/> containing retrieved data import entries.
        /// </returns>
        Task<QueryResponse<IEnumerable<CsvTemplateImportDto>>> GetAllImports();

        /// <summary>
        /// Retrieves a data import entry by ID.
        /// </summary>
        /// <param name="id">ID of the data import entry.</param>
        /// <returns>
        /// A task representing the asynchronous database query operation.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over the retrieved data import entry.
        /// </returns>
        Task<QueryResponse<CsvTemplateImportDto>> GetImport(Guid id);
    }
}