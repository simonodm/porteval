using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Services.Interfaces.Repositories
{
    public interface IDataImportRepository : IRepository
    {
        Task<IEnumerable<DataImport>> ListAllAsync();
        DataImport Add(DataImport import);
        DataImport Update(DataImport import);
        Task DeleteAsync(Guid id);
        void Delete(DataImport import);
        Task<bool> ExistsAsync(Guid id);
    }
}