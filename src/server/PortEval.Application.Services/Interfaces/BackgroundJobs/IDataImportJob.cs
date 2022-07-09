using PortEval.Domain.Models.Enums;
using System;
using System.Threading.Tasks;

namespace PortEval.Application.Services.Interfaces.BackgroundJobs
{
    public interface IDataImportJob
    {
        public Task Run(Guid importId, string inputFileName, TemplateType templateType, string logPath);
    }
}
