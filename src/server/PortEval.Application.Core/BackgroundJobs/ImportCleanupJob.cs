using System;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PortEval.Application.Core.Interfaces.BackgroundJobs;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Core.BackgroundJobs;

/// <inheritdoc cref="IImportCleanupJob" />
public class ImportCleanupJob : IImportCleanupJob
{
    private readonly IFileSystem _fileSystem;
    private readonly IDataImportRepository _importRepository;
    private readonly ILogger _logger;

    /// <summary>
    ///     Initializes the job.
    /// </summary>
    public ImportCleanupJob(IDataImportRepository importRepository, IFileSystem fileSystem,
        ILoggerFactory loggerFactory)
    {
        _importRepository = importRepository;
        _fileSystem = fileSystem;
        _logger = loggerFactory.CreateLogger(typeof(ImportCleanupJob));
    }

    /// <inheritdoc />
    public async Task RunAsync()
    {
        _logger.LogInformation("Import cleanup started.");

        var imports = await _importRepository.ListAllAsync();
        foreach (var import in imports)
            if (import.Time < DateTime.UtcNow.AddHours(-24))
                await DeleteImport(import);

        await _importRepository.UnitOfWork.CommitAsync();

        _logger.LogInformation("Import cleanup finished.");
    }

    private async Task DeleteImport(DataImport import)
    {
        await _importRepository.DeleteAsync(import.Id);
        if (_fileSystem.File.Exists(import.ErrorLogPath)) _fileSystem.File.Delete(import.ErrorLogPath);
    }
}