using System.Threading.Tasks;

namespace PortEval.Application.Features.Interfaces.BackgroundJobs
{
    public interface IImportCleanupJob
    {
        public Task Run();
    }
}
