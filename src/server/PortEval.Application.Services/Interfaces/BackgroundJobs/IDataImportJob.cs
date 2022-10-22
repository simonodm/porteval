using PortEval.Domain.Models.Entities;
using System.Threading.Tasks;

namespace PortEval.Application.Services.Interfaces.BackgroundJobs
{
    public interface IDataImportJob
    {
        public Task Run(DataImport importEntry, string inputFileName, string logPath);
    }
}
