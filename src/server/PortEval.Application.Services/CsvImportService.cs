using CsvHelper;
using Hangfire;
using PortEval.Application.Services.Extensions;
using PortEval.Application.Services.Interfaces;
using PortEval.Application.Services.Interfaces.BackgroundJobs;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace PortEval.Application.Services
{
    public class CsvImportService : ICsvImportService
    {
        private IDataImportRepository _importRepository;
        private readonly string _storagePath;

        public CsvImportService(IDataImportRepository importRepository, string storagePath)
        {
            _importRepository = importRepository;
            _storagePath = storagePath;
        }

        public async Task<DataImport> StartImport(Stream inputFileStream, TemplateType templateType)
        {
            var guid = Guid.NewGuid();

            var tempFilePath = GetTemporaryFilePath(guid);
            var logFilePath = GetErrorLogPath(guid);

            using var fs = new FileStream(tempFilePath, FileMode.Create);
            inputFileStream.CopyTo(fs);
            fs.Close();

            var importEntry = await SaveNewImportEntry(guid, templateType);
            BackgroundJob.Enqueue<IDataImportJob>(job => job.Run(importEntry, tempFilePath, logFilePath));

            return importEntry;
        }

        public Stream TryGetErrorLog(Guid guid)
        {
            try
            {
                return new FileStream(GetErrorLogPath(guid), FileMode.Open);
            }
            catch
            {
                return null;
            }
        }

        public Stream GetCsvTemplate(TemplateType templateType)
        {
            var path = GetTemplatePath(templateType);
            if(!File.Exists(path))
            {
                GenerateTemplate(path, templateType);
            }

            return new FileStream(path, FileMode.Open);
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
            using var sw = new StreamWriter(path);
            using var csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
            csv.RegisterImportClassMaps();

            var rowType = templateType.GetRowType();
            csv.WriteHeader(rowType);
            sw.Flush();
        }

        private async Task<DataImport> SaveNewImportEntry(Guid guid, TemplateType templateType)
        {
            var importEntry = new DataImport(guid, templateType, ImportStatus.InProgress);
            _importRepository.Add(importEntry);
            await _importRepository.UnitOfWork.CommitAsync();

            return importEntry;
        }
    }
}
