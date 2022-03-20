using System;
using System.IO;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs.Enums;

namespace PortEval.Application.Services.Interfaces
{
    public interface ICsvImportService
    {
        public Task ProcessUpload(Guid importId, Stream inputFileStream, CsvTemplateType templateType);
        public Stream TryGetErrorLog(Guid guid);
    }
}
