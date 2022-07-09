using Hangfire;
using PortEval.Application.Services.Interfaces;
using PortEval.Application.Services.Interfaces.BackgroundJobs;
using PortEval.Domain.Models.Enums;
using System;
using System.IO;

namespace PortEval.Application.Services
{
    public class CsvImportService : ICsvImportService
    {
        private readonly string _storagePath;

        public CsvImportService()
        {
            _storagePath = Environment.GetEnvironmentVariable("FILE_STORAGE");
        }

        public Guid StartImport(Stream inputFileStream, TemplateType templateType)
        {
            var guid = Guid.NewGuid();

            var tempFilePath = GetTemporaryFilePath(guid);
            var logFilePath = GetErrorLogPath(guid);
            using var fs = new FileStream(tempFilePath, FileMode.Create);
            inputFileStream.CopyTo(fs);
            fs.Close();

            BackgroundJob.Enqueue<IDataImportJob>(job => job.Run(guid, tempFilePath, templateType, logFilePath));

            return guid;
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

        private string GetTemporaryFilePath(Guid guid)
        {
            return _storagePath + (_storagePath[^1] == '/' ? "" : '/') + guid + "_import.csv";
        }

        private string GetErrorLogPath(Guid guid)
        {
            return _storagePath + (_storagePath[^1] == '/' ? "" : '/') + guid + "_log.csv";
        }
    }
}
