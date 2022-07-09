using System.Threading.Tasks;

namespace PortEval.Application.Services.Interfaces.BackgroundJobs
{
    public interface IImportCleanupJob
    {
        public Task Run();
    }
}
