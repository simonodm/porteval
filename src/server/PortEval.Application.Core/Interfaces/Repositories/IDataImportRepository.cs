using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Core.Interfaces.Repositories
{
    /// <summary>
    /// Represents a persistently stored collection of data import entries.
    /// </summary>
    public interface IDataImportRepository : IRepository
    {
        /// <summary>
        /// Retrieves all data import entries.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="IEnumerable{T}"/> containing all data import entries.
        /// </returns>
        Task<IEnumerable<DataImport>> ListAllAsync();

        /// <summary>
        /// Retrieves a data import by ID.
        /// </summary>
        /// <param name="id">ID of the data import to retrieve.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains a <see cref="DataImport"/> with a matching ID if it exists, <c>null</c> otherwise.
        /// </returns>
        Task<DataImport> FindAsync(Guid id);

        /// <summary>
        /// Adds a data import to the collection.
        /// </summary>
        /// <param name="import">Data import to add.</param>
        /// <returns>The added data import.</returns>
        DataImport Add(DataImport import);

        /// <summary>
        /// Updates a data import in the collection.
        /// </summary>
        /// <param name="import">Data import to update.</param>
        /// <returns>The updated data import.</returns>
        DataImport Update(DataImport import);

        /// <summary>
        /// Deletes a data import by ID.
        /// </summary>
        /// <param name="id">ID of the data import to delete.</param>
        /// <returns>
        /// A task representing the asynchronous deletion operation.
        /// </returns>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Deletes a data import.
        /// </summary>
        /// <param name="import">Data import to delete.</param>
        void Delete(DataImport import);

        /// <summary>
        /// Checks whether an import with the specified ID exists.
        /// </summary>
        /// <param name="id">ID of the import to validate existence of.</param>
        /// <returns>
        /// A task representing the asynchronous check operation.
        /// Task result contains <c>true</c> if an import with the specified ID exists, <c>false</c> otherwise.
        /// </returns>
        Task<bool> ExistsAsync(Guid id);
    }
}