using System.Threading.Tasks;

namespace PortEval.Application.Features.Interfaces.BackgroundJobs
{
    public interface ISplitFetchJob
    {
        public Task Run();
    }
}
