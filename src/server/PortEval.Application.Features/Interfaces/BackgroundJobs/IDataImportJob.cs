using System;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Interfaces.BackgroundJobs
{
    public interface IDataImportJob
    {
        public Task Run(Guid importId, string inputFileName, string logPath);
    }
}
