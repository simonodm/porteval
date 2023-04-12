using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.Services
{
    /// <summary>
    /// Handles operations related to CSV import.
    /// </summary>
    public interface ICsvImportService
    {
        /// <summary>
        /// Retrieves all data import entries.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing an <see cref="IEnumerable{T}"/> containing retrieved data import entries.
        /// </returns>
        public Task<OperationResponse<IEnumerable<CsvTemplateImportDto>>> GetAllImportsAsync();

        /// <summary>
        /// Retrieves a data import entry by ID.
        /// </summary>
        /// <param name="id">ID of the data import entry.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing the retrieved data import entry.
        /// </returns>
        public Task<OperationResponse<CsvTemplateImportDto>> GetImportAsync(Guid id);

        /// <summary>
        /// Creates and schedules a CSV import.
        /// </summary>
        /// <param name="inputFileStream">Input CSV file stream.</param>
        /// <param name="templateType">Type of data provided in the CSV file.</param>
        /// <returns>
        /// A task representing the asynchronous creation and scheduling operations.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing the created <see cref="DataImport"/> entry.
        /// </returns>
        public Task<OperationResponse<CsvTemplateImportDto>> StartImportAsync(Stream inputFileStream, TemplateType templateType);

        /// <summary>
        /// Attempts to retrieve an error log for a data import with the specified ID.
        /// </summary>
        /// <param name="guid">ID of the data import entry to retrieve error log of.</param>
        /// <returns>An <see cref="OperationResponse{T}"/> containing a stream to the import's error log if it exists, <c>null</c> otherwise.</returns>
        public OperationResponse<Stream> TryGetErrorLog(Guid guid);

        /// <summary>
        /// Retrieves the template file for the specified type of data.
        /// </summary>
        /// <param name="templateType">Type of data to retrieve template for.</param>
        /// <returns>An <see cref="OperationResponse{T}"/> containing a stream to the template file.</returns>
        public OperationResponse<Stream> GetCsvTemplate(TemplateType templateType);
    }
}
