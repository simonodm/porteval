using CsvHelper;
using Hangfire;
using PortEval.Application.Features.Extensions;
using PortEval.Application.Features.Interfaces.BackgroundJobs;
using PortEval.Application.Features.Interfaces.Repositories;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using System;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Services
{
    public class CsvImportService : ICsvImportService
    {
        private IDataImportRepository _importRepository;
        private IBackgroundJobClient _jobClient;
        private IFileSystem _fileSystem;
        private readonly string _storagePath;

        public CsvImportService(IDataImportRepository importRepository, IBackgroundJobClient jobClient, IFileSystem fileSystem, string storagePath)
        {
            _importRepository = importRepository;
            _jobClient = jobClient;
            _fileSystem = fileSystem;
            _storagePath = storagePath;
        }

        public async Task<DataImport> StartImport(Stream inputFileStream, TemplateType templateType)
        {
            var guid = Guid.NewGuid();

            var tempFilePath = GetTemporaryFilePath(guid);
            var logFilePath = GetErrorLogPath(guid);

            await using var fs = _fileSystem.FileStream.Create(tempFilePath, FileMode.Create);
            await inputFileStream.CopyToAsync(fs);
            fs.Close();

            var importEntry = await SaveNewImportEntry(guid, templateType);
            _jobClient.Enqueue<IDataImportJob>(job => job.Run(guid, tempFilePath, logFilePath));

            return importEntry;
        }

        public Stream TryGetErrorLog(Guid guid)
        {
            try
            {
                return _fileSystem.FileStream.Create(GetErrorLogPath(guid), FileMode.Open);
            }
            catch
            {
                return null;
            }
        }

        public Stream GetCsvTemplate(TemplateType templateType)
        {
            var path = GetTemplatePath(templateType);
            if (!_fileSystem.File.Exists(path))
            {
                GenerateTemplate(path, templateType);
            }

            return _fileSystem.FileStream.Create(path, FileMode.Open);
        }

        private string GetTemporaryFilePath(Guid guid)
        {
            return Path.Combine(_storagePath, guid + "_import.csv");
        }

        private string GetErrorLogPath(Guid guid)
        {
            return Path.Combine(_storagePath, guid + "_log.csv");
        }

        private string GetTemplatePath(TemplateType templateType)
        {
            return Path.Combine(_storagePath, templateType.ToString().ToLower() + "_template.csv");
        }

        private void GenerateTemplate(string path, TemplateType templateType)
        {
            using var fs = _fileSystem.FileStream.Create(path, FileMode.Create);
            using var sw = new StreamWriter(fs);
            using var csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
            csv.RegisterImportClassMaps();

            var rowType = templateType.GetRowType();
            csv.WriteHeader(rowType);
            sw.Flush();
        }

        private async Task<DataImport> SaveNewImportEntry(Guid guid, TemplateType templateType)
        {
            var importEntry = DataImport.Create(guid, DateTime.UtcNow, templateType, ImportStatus.Pending);
            _importRepository.Add(importEntry);
            await _importRepository.UnitOfWork.CommitAsync();

            return importEntry;
        }
    }
}
