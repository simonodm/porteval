using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Interfaces.Queries;

/// <summary>
///     Implements queries for bulk data imports stored in the application's persistent storage.
/// </summary>
public interface IDataImportQueries
{
    /// <summary>
    ///     Retrieves all created bulk imports.
    /// </summary>
    /// <returns>
    ///     A task representing the asynchronous query operation.
    ///     Task result contains an <see cref="IEnumerable{T}" /> containing all stored bulk imports.
    /// </returns>
    Task<IEnumerable<CsvTemplateImportDto>> GetAllImportsAsync();

    /// <summary>
    ///     Retrieves a bulk import by ID.
    /// </summary>
    /// <param name="id">ID of the import to retrieve.</param>
    /// <returns>
    ///     A task representing the asynchronous query operation.
    ///     Task result contains the retrieved bulk import if it exists, <c>null</c> otherwise.
    /// </returns>
    Task<CsvTemplateImportDto> GetImportAsync(Guid id);
}