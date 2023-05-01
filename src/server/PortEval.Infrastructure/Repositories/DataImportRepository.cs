using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;

namespace PortEval.Infrastructure.Repositories;

/// <inheritdoc cref="IDataImportRepository" />
public class DataImportRepository : IDataImportRepository
{
    private readonly PortEvalDbContext _context;

    public DataImportRepository(PortEvalDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public IUnitOfWork UnitOfWork => _context;

    /// <inheritdoc />
    public async Task<IEnumerable<DataImport>> ListAllAsync()
    {
        return await _context.Imports.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<DataImport> FindAsync(Guid id)
    {
        return await _context.Imports.FirstOrDefaultAsync(import => import.Id == id);
    }

    /// <inheritdoc />
    public DataImport Add(DataImport import)
    {
        return _context.Imports.Add(import).Entity;
    }

    /// <inheritdoc />
    public DataImport Update(DataImport import)
    {
        return _context.Imports.Update(import).Entity;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        var importEntry = await _context.Imports.FirstOrDefaultAsync(i => i.Id == id);
        if (importEntry != null)
        {
            Delete(importEntry);
        }
    }

    /// <inheritdoc />
    public void Delete(DataImport import)
    {
        _context.Imports.Remove(import);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Imports.AnyAsync(i => i.Id == id);
    }
}