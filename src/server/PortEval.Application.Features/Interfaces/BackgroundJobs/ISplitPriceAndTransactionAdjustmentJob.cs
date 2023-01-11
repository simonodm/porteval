using System.Threading.Tasks;

namespace PortEval.Application.Features.Interfaces.BackgroundJobs
{
    public interface ISplitPriceAndTransactionAdjustmentJob
    {
        public Task Run();
    }
}
