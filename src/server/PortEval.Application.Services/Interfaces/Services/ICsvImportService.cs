using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Interfaces.Services
{
    public interface ICsvImportService
    {
        public Task<DataImport> StartImport(Stream inputFileStream, TemplateType templateType);
        public Stream TryGetErrorLog(Guid guid);
        public Stream GetCsvTemplate(TemplateType templateType);
    }
}
