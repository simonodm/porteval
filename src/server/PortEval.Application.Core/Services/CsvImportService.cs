using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using CsvHelper;
using Hangfire;
using PortEval.Application.Core.Extensions;
using PortEval.Application.Core.Interfaces.BackgroundJobs;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.Services
{
    /// <inheritdoc cref="ICsvImportService" />
    public class CsvImportService : ICsvImportService
    {
        private readonly IDataImportRepository _importRepository;
        private readonly IDataImportQueries _importDataQueries;

        private readonly IBackgroundJobClient _jobClient;
        private readonly IFileSystem _fileSystem;
        private readonly IFileStreamFactory _fileStreamFactory;
        private readonly string _storagePath;

        public CsvImportService(IDataImportRepository importRepository, IBackgroundJobClient jobClient, IFileSystem fileSystem, IFileStreamFactory fileStreamFactory, IDataImportQueries importDataQueries, string storagePath)
        {
            _importRepository = importRepository;
            _jobClient = jobClient;
            _fileSystem = fileSystem;
            _storagePath = storagePath;
            _importDataQueries = importDataQueries;
            _fileStreamFactory = fileStreamFactory;
        }

        /// <inheritdoc />
        public async Task<OperationResponse<IEnumerable<CsvTemplateImportDto>>> GetAllImportsAsync()
        {
            var imports = await _importDataQueries.GetAllImportsAsync();

            return new OperationResponse<IEnumerable<CsvTemplateImportDto>>
            {
                Response = imports
            };
        }

        /// <inheritdoc />
        public async Task<OperationResponse<CsvTemplateImportDto>> GetImportAsync(Guid id)
        {
            var import = await _importDataQueries.GetImportAsync(id);

            return new OperationResponse<CsvTemplateImportDto>
            {
                Status = import != null ? OperationStatus.Ok : OperationStatus.NotFound,
                Message = import != null ? "" : $"Import {id} does not exist.",
                Response = import
            };
        }

        /// <inheritdoc />
        public async Task<OperationResponse<CsvTemplateImportDto>> StartImportAsync(Stream inputFileStream, TemplateType templateType)
        {
            var guid = Guid.NewGuid();

            var tempFilePath = GetTemporaryFilePath(guid);
            var logFilePath = GetErrorLogPath(guid);

            await using var fs = _fileStreamFactory.New(tempFilePath, FileMode.Create);
            await inputFileStream.CopyToAsync(fs);
            fs.Close();

            var importEntry = await SaveNewImportEntry(guid, templateType);
            _jobClient.Enqueue<IDataImportJob>(job => job.RunAsync(guid, tempFilePath, logFilePath));

            return await GetImportAsync(importEntry.Id);
        }

        /// <inheritdoc />
        public OperationResponse<Stream> TryGetErrorLog(Guid guid)
        {
            try
            {
                var stream = _fileStreamFactory.New(GetErrorLogPath(guid), FileMode.Open);
                return new OperationResponse<Stream>
                {
                    Response = stream
                };
            }
            catch
            {
                return new OperationResponse<Stream>
                {
                    Status = OperationStatus.Error,
                    Message = $"Failed to retrieve error log for ID {guid}"
                };
            }
        }

        /// <inheritdoc />
        public OperationResponse<Stream> GetCsvTemplate(TemplateType templateType)
        {
            var path = GetTemplatePath(templateType);
            if (!_fileSystem.File.Exists(path))
            {
                GenerateTemplate(path, templateType);
            }

            return new OperationResponse<Stream>
            {
                Response = _fileStreamFactory.New(path, FileMode.Open)
            };
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
            using var fs = _fileStreamFactory.New(path, FileMode.Create);
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
