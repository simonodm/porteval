using System.Collections.Generic;

namespace PortEval.Application.Core.Interfaces.Services
{
    /// <summary>
    /// Handles CSV export operations.
    /// </summary>
    public interface ICsvExportService
    {
        /// <summary>
        /// Converts the provided records to CSV format.
        /// </summary>
        /// <typeparam name="T">Type of provided records.</typeparam>
        /// <param name="rows">A collection of records to convert to CSV.</param>
        /// <returns>A byte array representing the final CSV file.</returns>
        public byte[] ConvertToCsv<T>(IEnumerable<T> rows);
    }
}
