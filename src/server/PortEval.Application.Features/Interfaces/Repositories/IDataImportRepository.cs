using PortEval.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Interfaces.Repositories
{
    public interface IDataImportRepository : IRepository
    {
        Task<IEnumerable<DataImport>> ListAllAsync();
        Task<DataImport> FindAsync(Guid id);
        DataImport Add(DataImport import);
        DataImport Update(DataImport import);
        Task DeleteAsync(Guid id);
        void Delete(DataImport import);
        Task<bool> ExistsAsync(Guid id);
    }
}