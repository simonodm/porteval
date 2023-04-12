using Microsoft.EntityFrameworkCore;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Infrastructure.Repositories
{
    public class DataImportRepository : IDataImportRepository
    {
        public IUnitOfWork UnitOfWork => _context;
        private PortEvalDbContext _context;

        public DataImportRepository(PortEvalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DataImport>> ListAllAsync()
        {
            return await _context.Imports.ToListAsync();
        }

        public async Task<DataImport> FindAsync(Guid id)
        {
            return await _context.Imports.FirstOrDefaultAsync(import => import.Id == id);
        }

        public DataImport Add(DataImport import)
        {
            return _context.Imports.Add(import).Entity;
        }

        public DataImport Update(DataImport import)
        {
            return _context.Imports.Update(import).Entity;
        }

        public async Task DeleteAsync(Guid id)
        {
            var importEntry = await _context.Imports.FirstOrDefaultAsync(i => i.Id == id);
            if (importEntry != null)
            {
                Delete(importEntry);
            }
        }

        public void Delete(DataImport import)
        {
            _context.Imports.Remove(import);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Imports.AnyAsync(i => i.Id == id);
        }
    }
}
