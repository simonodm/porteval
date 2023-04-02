using System;
using System.IO;
using System.Threading.Tasks;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.Interfaces.Services
{
    /// <summary>
    /// Handles operations related to CSV import.
    /// </summary>
    public interface ICsvImportService
    {
        /// <summary>
        /// Creates and schedules a CSV import.
        /// </summary>
        /// <param name="inputFileStream">Input CSV file stream.</param>
        /// <param name="templateType">Type of data provided in the CSV file.</param>
        /// <returns>
        /// A task representing the asynchronous creation and scheduling operations.
        /// Task result contains the created <see cref="DataImport"/> entry.
        /// </returns>
        public Task<DataImport> StartImport(Stream inputFileStream, TemplateType templateType);

        /// <summary>
        /// Attempts to retrieve an error log for a data import with the specified ID.
        /// </summary>
        /// <param name="guid">ID of the data import entry to retrieve error log of.</param>
        /// <returns>A stream to the import's error log if it exists, <c>null</c> otherwise.</returns>
        public Stream TryGetErrorLog(Guid guid);

        /// <summary>
        /// Retrieves the template file for the specified type of data.
        /// </summary>
        /// <param name="templateType">Type of data to retrieve template for.</param>
        /// <returns>A stream to the template file.</returns>
        public Stream GetCsvTemplate(TemplateType templateType);
    }
}
