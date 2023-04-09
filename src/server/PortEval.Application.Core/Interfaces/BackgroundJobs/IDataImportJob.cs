using System;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.BackgroundJobs
{
    /// <summary>
    /// A job responsible for processing a bulk CSV import by its GUID.
    /// </summary>
    public interface IDataImportJob
    {
        /// <summary>
        /// Executes the job.
        /// </summary>
        /// <param name="importId">ID of the import entry.</param>
        /// <param name="inputFileName">Path to the CSV file to be imported.</param>
        /// <param name="logPath">Path to the log file.</param>
        /// <returns>A task representing the asynchronous import operation.</returns>
        public Task RunAsync(Guid importId, string inputFileName, string logPath);
    }
}
