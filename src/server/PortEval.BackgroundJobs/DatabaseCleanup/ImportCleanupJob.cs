using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PortEval.Application.Services.Interfaces.BackgroundJobs;
using PortEval.Application.Services.Interfaces.Repositories;

namespace PortEval.BackgroundJobs.DatabaseCleanup
{
    public class ImportCleanupJob : IImportCleanupJob
    {
        private readonly IDataImportRepository _importRepository;
        private readonly ILogger _logger;

        public ImportCleanupJob(IDataImportRepository importRepository, ILoggerFactory loggerFactory)
        {
            _importRepository = importRepository;
            _logger = loggerFactory.CreateLogger(typeof(ImportCleanupJob));
        }

        public async Task Run()
        {
            _logger.LogInformation($"Import cleanup started at {DateTime.UtcNow}.");

            var imports = await _importRepository.ListAllAsync();
            foreach (var import in imports)
            {
                if (import.Time < DateTime.Today - TimeSpan.FromDays(1))
                {
                    await DeleteImport(import);
                }
            }

            await _importRepository.UnitOfWork.CommitAsync();

            _logger.LogInformation($"Import cleanup finished at {DateTime.UtcNow}.");
        }

        private async Task DeleteImport(Domain.Models.Entities.DataImport import)
        {
            await _importRepository.DeleteAsync(import.Id);
            if (File.Exists(import.ErrorLogPath))
            {
                File.Delete(import.ErrorLogPath);
            }
        }
    }
}
