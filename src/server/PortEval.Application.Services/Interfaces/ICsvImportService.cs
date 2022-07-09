using PortEval.Domain.Models.Enums;
using System;
using System.IO;

namespace PortEval.Application.Services.Interfaces
{
    public interface ICsvImportService
    {
        public Guid StartImport(Stream inputFileStream, TemplateType templateType);
        public Stream TryGetErrorLog(Guid guid);
    }
}
