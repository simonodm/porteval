using PortEval.Application.Core.Common.BulkImportExport;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces
{
    /// <summary>
    /// Represents a bulk import processor.
    /// </summary>
    /// <typeparam name="TRecord">Type of imported records.</typeparam>
    public interface IImportProcessor<TRecord>
    {
        /// <summary>
        /// Imports the provided records into the application.
        /// </summary>
        /// <param name="rows">Records to import.</param>
        /// <returns>
        /// A task representing the asynchronous import operation.
        /// Task result contains an <see cref="ImportResult{T}"/> instance containing information about the processed import.
        /// </returns>
        public Task<ImportResult<TRecord>> ImportRecordsAsync(IEnumerable<TRecord> rows);
    }
}
